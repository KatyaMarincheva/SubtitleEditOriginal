// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlUtilTest.cs" company="">
//   
// </copyright>
// <summary>
//   The html util test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// <copyright file="HtmlUtilTest.cs" company="Nikse">Nikse</copyright>

namespace Nikse.SubtitleEdit.Core
{
    using System;
    using System.Reflection;

    using Microsoft.Pex.Framework;
    using Microsoft.Pex.Framework.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The html util test.
    /// </summary>
    [TestClass]
    [PexClass(typeof(HtmlUtil))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class HtmlUtilTest
    {
        /// <summary>
        /// The encode text.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        public string EncodeText(string source)
        {
            string result = HtmlUtil.EncodeText(source);
            Assert.IsNotNull(result, "Result cannot be null.");
            return result;
        }

        /// <summary>
        /// The remove html tags.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="alsoSsaTags">
        /// The also ssa tags.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod(MaxConstraintSolverTime = 2)]
        public string RemoveHtmlTags(string s, bool alsoSsaTags)
        {
            string result = HtmlUtil.RemoveHtmlTags(s, alsoSsaTags);
            return result;

            // TODO: add assertions to method HtmlUtilTest.RemoveHtmlTags(String, Boolean)
        }

        /// <summary>
        /// The is url.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [PexMethod]
        public bool IsUrl(string text)
        {
            bool result = HtmlUtil.IsUrl(text);
            return result;

            // TODO: add assertions to method HtmlUtilTest.IsUrl(String)
        }

        /// <summary>
        /// The starts with url.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [PexMethod]
        public bool StartsWithUrl(string text)
        {
            bool result = HtmlUtil.StartsWithUrl(text);
            return result;

            // TODO: add assertions to method HtmlUtilTest.StartsWithUrl(String)
        }

        /// <summary>
        /// The fix upper tags.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        public string FixUpperTags(string text)
        {
            string result = HtmlUtil.FixUpperTags(text);
            return result;

            // TODO: add assertions to method HtmlUtilTest.FixUpperTags(String)
        }

        /// <summary>
        /// The fix invalid italic tags.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        public string FixInvalidItalicTags(string text)
        {
            string result = HtmlUtil.FixInvalidItalicTags(text);
            return result;

            // TODO: add assertions to method HtmlUtilTest.FixInvalidItalicTags(String)
        }

        /// <summary>
        /// The remove multiple begin tags.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="italicBeginTagCount">
        /// The italic begin tag count.
        /// </param>
        /// <param name="italicEndTagCount">
        /// The italic end tag count.
        /// </param>
        /// <param name="beginTag">
        /// The begin tag.
        /// </param>
        /// <param name="endTag">
        /// The end tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("RemoveMultipleBeginTags(String, Int32, Int32, String, String)")]
        internal string RemoveMultipleBeginTags(string text, int italicBeginTagCount, int italicEndTagCount, string beginTag, string endTag)
        {
            object[] args = new object[5];
            args[0] = (object)text;
            args[1] = (object)italicBeginTagCount;
            args[2] = (object)italicEndTagCount;
            args[3] = (object)beginTag;
            args[4] = (object)endTag;
            Type[] parameterTypes = new Type[5];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(int);
            parameterTypes[2] = typeof(int);
            parameterTypes[3] = typeof(string);
            parameterTypes[4] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("RemoveMultipleBeginTags", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.RemoveMultipleBeginTags(String, Int32, Int32, String, String)
        }

        /// <summary>
        /// The remove italic begin end tag group.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("RemoveItalicBeginEndTagGroup(String)")]
        internal string RemoveItalicBeginEndTagGroup(string text)
        {
            object[] args = new object[1];
            args[0] = (object)text;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("RemoveItalicBeginEndTagGroup", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.RemoveItalicBeginEndTagGroup(String)
        }

        /// <summary>
        /// The set begin and end tag for 2 lines text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="italicBeginTagCount">
        /// The italic begin tag count.
        /// </param>
        /// <param name="italicEndTagCount">
        /// The italic end tag count.
        /// </param>
        /// <param name="beginTag">
        /// The begin tag.
        /// </param>
        /// <param name="endTag">
        /// The end tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("SetBeginAndEndTagFor2LinesText(String, Int32, Int32, String, String)")]
        internal string SetBeginAndEndTagFor2LinesText(string text, int italicBeginTagCount, int italicEndTagCount, string beginTag, string endTag)
        {
            object[] args = new object[5];
            args[0] = (object)text;
            args[1] = (object)italicBeginTagCount;
            args[2] = (object)italicEndTagCount;
            args[3] = (object)beginTag;
            args[4] = (object)endTag;
            Type[] parameterTypes = new Type[5];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(int);
            parameterTypes[2] = typeof(int);
            parameterTypes[3] = typeof(string);
            parameterTypes[4] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("SetBeginAndEndTagFor2LinesText", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.SetBeginAndEndTagFor2LinesText(String, Int32, Int32, String, String)
        }

        /// <summary>
        /// The two end tags convert first to begin tag.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="italicBeginTagCount">
        /// The italic begin tag count.
        /// </param>
        /// <param name="italicEndTagCount">
        /// The italic end tag count.
        /// </param>
        /// <param name="endTag">
        /// The end tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("TwoEndTagsConvertFirstToBeginTag(String, Int32, Int32, String)")]
        internal string TwoEndTagsConvertFirstToBeginTag(string text, int italicBeginTagCount, int italicEndTagCount, string endTag)
        {
            object[] args = new object[4];
            args[0] = (object)text;
            args[1] = (object)italicBeginTagCount;
            args[2] = (object)italicEndTagCount;
            args[3] = (object)endTag;
            Type[] parameterTypes = new Type[4];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(int);
            parameterTypes[2] = typeof(int);
            parameterTypes[3] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("TwoEndTagsConvertFirstToBeginTag", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.TwoEndTagsConvertFirstToBeginTag(String, Int32, Int32, String)
        }

        /// <summary>
        /// The two end tags set begin and end tags.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="italicBeginTagCount">
        /// The italic begin tag count.
        /// </param>
        /// <param name="italicEndTagCount">
        /// The italic end tag count.
        /// </param>
        /// <param name="endTag">
        /// The end tag.
        /// </param>
        /// <param name="beginTag">
        /// The begin tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("TwoEndTagsSetBeginAndEndTags(String, Int32, Int32, String, String)")]
        internal string TwoEndTagsSetBeginAndEndTags(string text, int italicBeginTagCount, int italicEndTagCount, string endTag, string beginTag)
        {
            object[] args = new object[5];
            args[0] = (object)text;
            args[1] = (object)italicBeginTagCount;
            args[2] = (object)italicEndTagCount;
            args[3] = (object)endTag;
            args[4] = (object)beginTag;
            Type[] parameterTypes = new Type[5];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(int);
            parameterTypes[2] = typeof(int);
            parameterTypes[3] = typeof(string);
            parameterTypes[4] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("TwoEndTagsSetBeginAndEndTags", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.TwoEndTagsSetBeginAndEndTags(String, Int32, Int32, String, String)
        }

        /// <summary>
        /// The add missing begin tag.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="italicBeginTagCount">
        /// The italic begin tag count.
        /// </param>
        /// <param name="italicEndTagCount">
        /// The italic end tag count.
        /// </param>
        /// <param name="endTag">
        /// The end tag.
        /// </param>
        /// <param name="beginTag">
        /// The begin tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("AddMissingBeginTag(String, Int32, Int32, String, String)")]
        internal string AddMissingBeginTag(string text, int italicBeginTagCount, int italicEndTagCount, string endTag, string beginTag)
        {
            object[] args = new object[5];
            args[0] = (object)text;
            args[1] = (object)italicBeginTagCount;
            args[2] = (object)italicEndTagCount;
            args[3] = (object)endTag;
            args[4] = (object)beginTag;
            Type[] parameterTypes = new Type[5];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(int);
            parameterTypes[2] = typeof(int);
            parameterTypes[3] = typeof(string);
            parameterTypes[4] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("AddMissingBeginTag", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.AddMissingBeginTag(String, Int32, Int32, String, String)
        }

        /// <summary>
        /// The add missing end tag.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="italicBeginTagCount">
        /// The italic begin tag count.
        /// </param>
        /// <param name="italicEndTagCount">
        /// The italic end tag count.
        /// </param>
        /// <param name="beginTag">
        /// The begin tag.
        /// </param>
        /// <param name="endTag">
        /// The end tag.
        /// </param>
        /// <param name="noOfLines">
        /// The no of lines.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("AddMissingEndTag(String, Int32, Int32, String, String, Int32)")]
        internal string AddMissingEndTag(string text, int italicBeginTagCount, int italicEndTagCount, string beginTag, string endTag, int noOfLines)
        {
            object[] args = new object[6];
            args[0] = (object)text;
            args[1] = (object)italicBeginTagCount;
            args[2] = (object)italicEndTagCount;
            args[3] = (object)beginTag;
            args[4] = (object)endTag;
            args[5] = (object)noOfLines;
            Type[] parameterTypes = new Type[6];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(int);
            parameterTypes[2] = typeof(int);
            parameterTypes[3] = typeof(string);
            parameterTypes[4] = typeof(string);
            parameterTypes[5] = typeof(int);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("AddMissingEndTag", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.AddMissingEndTag(String, Int32, Int32, String, String, Int32)
        }

        /// <summary>
        /// The remove duplicate begin tag.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="italicBeginTagCount">
        /// The italic begin tag count.
        /// </param>
        /// <param name="italicEndTagCount">
        /// The italic end tag count.
        /// </param>
        /// <param name="beginTag">
        /// The begin tag.
        /// </param>
        /// <param name="endTag">
        /// The end tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("RemoveDuplicateBeginTag(String, Int32, Int32, String, String)")]
        internal string RemoveDuplicateBeginTag(string text, int italicBeginTagCount, int italicEndTagCount, string beginTag, string endTag)
        {
            object[] args = new object[5];
            args[0] = (object)text;
            args[1] = (object)italicBeginTagCount;
            args[2] = (object)italicEndTagCount;
            args[3] = (object)beginTag;
            args[4] = (object)endTag;
            Type[] parameterTypes = new Type[5];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(int);
            parameterTypes[2] = typeof(int);
            parameterTypes[3] = typeof(string);
            parameterTypes[4] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("RemoveDuplicateBeginTag", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.RemoveDuplicateBeginTag(String, Int32, Int32, String, String)
        }

        /// <summary>
        /// The remove duplicate end tag.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="italicBeginTagCount">
        /// The italic begin tag count.
        /// </param>
        /// <param name="italicEndTagCount">
        /// The italic end tag count.
        /// </param>
        /// <param name="endTag">
        /// The end tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("RemoveDuplicateEndTag(String, Int32, Int32, String)")]
        internal string RemoveDuplicateEndTag(string text, int italicBeginTagCount, int italicEndTagCount, string endTag)
        {
            object[] args = new object[4];
            args[0] = (object)text;
            args[1] = (object)italicBeginTagCount;
            args[2] = (object)italicEndTagCount;
            args[3] = (object)endTag;
            Type[] parameterTypes = new Type[4];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(int);
            parameterTypes[2] = typeof(int);
            parameterTypes[3] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("RemoveDuplicateEndTag", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.RemoveDuplicateEndTag(String, Int32, Int32, String)
        }

        /// <summary>
        /// The if two begin tags convert second to end tag.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="italicBeginTagCount">
        /// The italic begin tag count.
        /// </param>
        /// <param name="italicEndTagCount">
        /// The italic end tag count.
        /// </param>
        /// <param name="beginTag">
        /// The begin tag.
        /// </param>
        /// <param name="noOfLines">
        /// The no of lines.
        /// </param>
        /// <param name="endTag">
        /// The end tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("IfTwoBeginTagsConvertSecondToEndTag(String, Int32, Int32, String, Int32, String)")]
        internal string IfTwoBeginTagsConvertSecondToEndTag(string text, int italicBeginTagCount, int italicEndTagCount, string beginTag, int noOfLines, string endTag)
        {
            object[] args = new object[6];
            args[0] = (object)text;
            args[1] = (object)italicBeginTagCount;
            args[2] = (object)italicEndTagCount;
            args[3] = (object)beginTag;
            args[4] = (object)noOfLines;
            args[5] = (object)endTag;
            Type[] parameterTypes = new Type[6];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(int);
            parameterTypes[2] = typeof(int);
            parameterTypes[3] = typeof(string);
            parameterTypes[4] = typeof(int);
            parameterTypes[5] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("IfTwoBeginTagsConvertSecondToEndTag", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.IfTwoBeginTagsConvertSecondToEndTag(String, Int32, Int32, String, Int32, String)
        }

        /// <summary>
        /// The swap end and begin tags.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="italicBeginTagCount">
        /// The italic begin tag count.
        /// </param>
        /// <param name="italicEndTagCount">
        /// The italic end tag count.
        /// </param>
        /// <param name="beginTag">
        /// The begin tag.
        /// </param>
        /// <param name="endTag">
        /// The end tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("SwapEndAndBeginTags(String, Int32, Int32, String, String)")]
        internal string SwapEndAndBeginTags(string text, int italicBeginTagCount, int italicEndTagCount, string beginTag, string endTag)
        {
            object[] args = new object[5];
            args[0] = (object)text;
            args[1] = (object)italicBeginTagCount;
            args[2] = (object)italicEndTagCount;
            args[3] = (object)beginTag;
            args[4] = (object)endTag;
            Type[] parameterTypes = new Type[5];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(int);
            parameterTypes[2] = typeof(int);
            parameterTypes[3] = typeof(string);
            parameterTypes[4] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("SwapEndAndBeginTags", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.SwapEndAndBeginTags(String, Int32, Int32, String, String)
        }

        /// <summary>
        /// The remove adjacent duplicate end tags.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="endTag">
        /// The end tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("RemoveAdjacentDuplicateEndTags(String, String)")]
        internal string RemoveAdjacentDuplicateEndTags(string text, string endTag)
        {
            object[] args = new object[2];
            args[0] = (object)text;
            args[1] = (object)endTag;
            Type[] parameterTypes = new Type[2];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("RemoveAdjacentDuplicateEndTags", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.RemoveAdjacentDuplicateEndTags(String, String)
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="beginTag">
        /// The begin tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("RemoveAdjacentDuplicateBeginTags(String, String)")]
        internal string RemoveAdjacentDuplicateBeginTags(string text, string beginTag)
        {
            object[] args = new object[2];
            args[0] = (object)text;
            args[1] = (object)beginTag;
            Type[] parameterTypes = new Type[2];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("RemoveAdjacentDuplicateBeginTags", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.RemoveAdjacentDuplicateBeginTags(String, String)
        }

        /// <summary>
        /// The remove unnecessary self closing tags.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="beginTag">
        /// The begin tag.
        /// </param>
        /// <param name="endTag">
        /// The end tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("RemoveUnnecessarySelfClosingTags(String, String, String)")]
        internal string RemoveUnnecessarySelfClosingTags(string text, string beginTag, string endTag)
        {
            object[] args = new object[3];
            args[0] = (object)text;
            args[1] = (object)beginTag;
            args[2] = (object)endTag;
            Type[] parameterTypes = new Type[3];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(string);
            parameterTypes[2] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("RemoveUnnecessarySelfClosingTags", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.RemoveUnnecessarySelfClosingTags(String, String, String)
        }

        /// <summary>
        /// The remove italic end begin tag groups.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("RemoveItalicEndBeginTagGroups(String)")]
        internal string RemoveItalicEndBeginTagGroups(string text)
        {
            object[] args = new object[1];
            args[0] = (object)text;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("RemoveItalicEndBeginTagGroups", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.RemoveItalicEndBeginTagGroups(String)
        }

        /// <summary>
        /// The fix end tags.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="endTag">
        /// The end tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("FixEndTags(String, String)")]
        internal string FixEndTags(string text, string endTag)
        {
            object[] args = new object[2];
            args[0] = (object)text;
            args[1] = (object)endTag;
            Type[] parameterTypes = new Type[2];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("FixEndTags", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.FixEndTags(String, String)
        }

        /// <summary>
        /// The fix begin tags.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="beginTag">
        /// The begin tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("FixBeginTags(String, String)")]
        internal string FixBeginTags(string text, string beginTag)
        {
            object[] args = new object[2];
            args[0] = (object)text;
            args[1] = (object)beginTag;
            Type[] parameterTypes = new Type[2];
            parameterTypes[0] = typeof(string);
            parameterTypes[1] = typeof(string);
            string result0 = ((MethodBase)typeof(HtmlUtil).GetMethod("FixBeginTags", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method HtmlUtilTest.FixBeginTags(String, String)
        }
    }
}