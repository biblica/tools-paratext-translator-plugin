/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Microsoft.ClearScript.V8;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TvpMain.Check
{
    /// <summary>
    /// This is the main check/fix runner
    /// Given the text to check, and the check/fix model, it'll run the check/fix against the text,
    /// returning the result set.
    /// </summary>
    public class CheckAndFixRunner : ICheckAndFixRunner
    {
        /// <summary>
        /// Default regex options.
        /// </summary>
        private const RegexOptions DEFAULT_REGEX_OPTIONS = (RegexOptions.Multiline | RegexOptions.ECMAScript);

        /// <summary>
        /// Escape code regex.
        ///
        /// Note: This needs to be an or-block instead of a list of regexes, otherwise there will be overlapping matches on the same text
        /// since USFM escape codes all begin with "\".
        /// </summary>
        private static readonly Regex EscapeCodeRegex = new Regex(@"(\\\\|\\r|\\n|\\t|\\u[0-9A-Fa-f]{4})", DEFAULT_REGEX_OPTIONS);

        /// <summary>
        /// Executes the check/fix against the given text
        /// </summary>
        /// <param name="inputText">The inputText (BCV) to be checked.</param>
        /// <param name="checkAndFixItem">The model that describes the check/fix</param>
        /// <returns>The list of check result items. This includes the suggested fixes if they are available.</returns>
        public List<CheckResultItem> ExecCheckAndFix(string inputText, CheckAndFixItem checkAndFixItem)
        {
            // Ensure item has required regular expression for performing the check.
            if(string.IsNullOrEmpty(checkAndFixItem.CheckRegex))
            {
                throw new CheckAndFixException("Missing expected Check regular expression", checkAndFixItem);
            }

            // First, run the check regex, looking for matches
            Regex checkRegex;
            try
            {
                checkRegex = new Regex(checkAndFixItem.CheckRegex, DEFAULT_REGEX_OPTIONS);
            }
            catch (Exception)
            {
                throw new CheckAndFixException("Invalid Check regular expression.", checkAndFixItem);
            }
            var fixRegex = EscapeCodeRegex.Replace(checkAndFixItem.FixRegex ?? string.Empty,
                foundMatch => Regex.Unescape(foundMatch.Value));
            var checkResultItems = new List<CheckResultItem>();

            // Now, loop through the matches
            foreach (Match matchItem in checkRegex.Matches(inputText))
            {
                // Create a result item
                var checkResultItem = new CheckResultItem(checkAndFixItem.Description,
                    matchItem.Value,
                    matchItem.Index,
                    CheckType.MissingSentencePunctuation,
                    (int)ScriptureReferenceErrorType.LooseFormatting
                    );

                // If there is a replacement regex, apply that to the result
                if (!string.IsNullOrEmpty(fixRegex))
                {
                    checkResultItem.FixText = matchItem.Result(fixRegex);
                }
                checkResultItems.Add(checkResultItem);
            }

            // if we have a script to run, run it
            if (string.IsNullOrEmpty(checkAndFixItem.CheckScript))
            {
                return checkResultItems;
            }

            // start up the V8 engine to run the script
            // can set various flags
            // See [V8ScriptEngineFlags] The most interesting are
            // V8ScriptEngineFlags.EnableRemoteDebugging - for remote debugging using something like VS Code
            // V8ScriptEngineFlags.EnableDebugging - again, enable debugging
            // V8ScriptEngineFlags.AwaitDebuggerAndPauseOnStart - used to allow the debugger to attach as soon 
            // as the engine is set for the script
            using var engine = new V8ScriptEngine();

            // passing data into and out of the script is done by JSON to limit issues with conversions
            var resultsString = JsonConvert.SerializeObject(checkResultItems);

            // This entry script demarshals the input and marshals the response
            // The script in the check/fix need only have the 'checkAndFix' function defined
            var entryScript = @"
                        function entry(jsonCheckResultItems) {
                            let checkResultItems = JSON.parse(jsonCheckResultItems)
                            return JSON.stringify(checkAndFix(checkResultItems))
                        }
                    ";

            // Here we can add a bunch of different data and bind context variables
            // For example, this is where we'd bind project settings
            // Right now, just the console
            engine.AddHostType("Console", typeof(Console));
            // execute the script
            engine.Execute(@checkAndFixItem.CheckScript + entryScript);
            // convert the response to json
            var outResultItems = engine.Script.entry(resultsString);
            checkResultItems = JsonConvert.DeserializeObject<List<CheckResultItem>>(outResultItems);

            // return the results
            return checkResultItems;
        }
    }

    /// <summary>
    /// Scripture reference error sub-types,
    /// used for ResultTypeCode field in ResultItem.
    /// </summary>
    public enum ScriptureReferenceErrorType
    {
        LooseFormatting, // Spacing or punctuation misses
        IncorrectNameStyle, // Name style inappropriate for context (e.g., abbrev when it should be long).
        TagShouldNotExist, // for when there shouldn't be a tag
                           // (found in section that should not
                           // have tags (the other ignore list -- overloaded term) )
        MissingTag, // Reference should have tag, but does not (e.g., in body footnote)
        IncorrectTag, // Wrong tag for context (e.g., \xt in footnote instead of \+xt)
        MalformedTag, // Missing slash(es), unmatched tag pair, etc.
        BadReference, // Book name or chapter number incorrect or doesn't exist
        Unknown
    }

}
