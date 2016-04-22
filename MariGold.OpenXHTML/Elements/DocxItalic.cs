﻿namespace MariGold.OpenXHTML
{
    using System;
    using MariGold.HtmlParser;
    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Wordprocessing;

    internal sealed class DocxItalic : DocxElement
    {
        internal DocxItalic(IOpenXmlContext context)
            : base(context)
        {
        }

        internal override bool CanConvert(IHtmlNode node)
        {
            return string.Compare(node.Tag, "i", StringComparison.InvariantCultureIgnoreCase) == 0 ||
            string.Compare(node.Tag, "em", StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        internal override void Process(DocxProperties properties, ref Paragraph paragraph)
        {
            if (properties.CurrentNode == null)
            {
                return;
            }

            foreach (IHtmlNode child in properties.CurrentNode.Children)
            {
                if (child.IsText)
                {
                    if (!IsEmptyText(child.InnerHtml))
                    {
                        if (paragraph == null)
                        {
                            paragraph = properties.Parent.AppendChild(new Paragraph());
                            ParagraphCreated(properties.ParagraphNode, paragraph);
                        }

                        Run run = paragraph.AppendChild(new Run());
                        RunCreated(properties.CurrentNode, run);

                        if (run.RunProperties == null)
                        {
                            run.RunProperties = new RunProperties();
                        }

                        DocxFont.ApplyFontItalic(run.RunProperties);

                        run.AppendChild(new Text()
                        {
                            Text = ClearHtml(child.InnerHtml),
                            Space = SpaceProcessingModeValues.Preserve
                        });
                    }
                }
                else
                {
                    ProcessChild(new DocxProperties(child, properties.ParagraphNode, properties.Parent), ref paragraph);
                }
            }
        }
    }
}
