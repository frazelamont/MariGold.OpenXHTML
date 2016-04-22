﻿namespace MariGold.OpenXHTML
{
	using System;
    using System.Text.RegularExpressions;
	using MariGold.HtmlParser;
	using DocumentFormat.OpenXml;
	using DocumentFormat.OpenXml.Wordprocessing;
	
	internal abstract class DocxElement
	{
		protected readonly IOpenXmlContext context;
		
		protected void RunCreated(IHtmlNode node, Run run)
		{
			DocxRunStyle style = new DocxRunStyle();
			style.Process(run, node);
		}
		
		protected void ParagraphCreated(IHtmlNode node, Paragraph para)
		{
			DocxParagraphStyle style = new DocxParagraphStyle();
			style.Process(para, node);
		}

        protected void ProcessChild(DocxProperties properties, ref Paragraph paragraph)
		{
            DocxElement element = context.Convert(properties.CurrentNode);
					
			if (element != null)
			{
                element.Process(properties, ref paragraph);
			}
		}

        protected void ProcessTextElement(DocxProperties properties)
		{
            ITextElement element = context.ConvertTextElement(properties.CurrentNode);
			
			if (element != null)
			{
                element.Process(properties);
			}
		}
		
		internal DocxElement(IOpenXmlContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			
			this.context = context;
		}
		
		internal string ClearHtml(string html)
		{
			if (string.IsNullOrEmpty(html))
			{
				return string.Empty;
			}
			
			html = html.Replace("&nbsp;", " ");
			html = html.Replace("&amp;", "&");

            Regex regex = new Regex(Environment.NewLine + "\\s+");
            Match match = regex.Match(html);

            while (match.Success)
            {
                //match.Length - 1 for leave a single space. Otherwise the sentences will collide.
                html = html.Remove(match.Index, match.Length - 1);
                match = regex.Match(html);
            }

            html = html.Replace(Environment.NewLine, string.Empty);

            return html;
		}
		
		internal bool IsEmptyText(string html)
		{
			if (string.IsNullOrEmpty(html))
			{
				return true;
			}
			
			html = html.Replace(Environment.NewLine, string.Empty);
			
			if (string.IsNullOrEmpty(html.Trim()))
			{
				return true;
			}
			
			return false;
		}
		
		internal abstract bool CanConvert(IHtmlNode node);
		
		internal abstract void Process(DocxProperties properties, ref Paragraph paragraph);
	}
}
