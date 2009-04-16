using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace AutoGen.TeXML
{
    public enum TeXTypes
    {
        TeXDoc = -1,
        TeXElement,
        TeXRoot,
        TeXCmd,
        TeXCmdOpt,
        TeXCmdAtt
    }

    public enum TeXModes
    {
        Text,
        Math
    }

    public class TeXMLDocList : List<TeXMLDoc>
    {
        public TeXMLDocList(IEnumerable<TeXMLDoc> collection) : base(collection)
        {
        }

        public TeXMLDocList(int capacity) : base(capacity)
        {
        }

        public TeXMLDocList()
        {
        }

        public new TeXMLDocList Add(TeXMLDoc doc)
        {
            base.Add(doc);
            return this;
        }
    }

    public class TeXElementCollection : List<TeXElement>
    {
        private readonly TeXElement _Parent;

        public TeXElement Parent
        {
            get { return _Parent; }
        }

        public TeXElementCollection(IEnumerable<TeXElement> collection, TeXElement parent)
            : base(collection)
        {
            _Parent = parent;
        }

        public TeXElementCollection(int capacity, TeXElement parent)
            : base(capacity)
        {
            _Parent = parent;
        }

        public TeXElementCollection(TeXElement parent)
        {
            _Parent = parent;
        }
    }

    public class TeXAttributes : Dictionary<string, string>
    {
        private readonly TeXElement _Element;

        public TeXElement Element
        {
            get { return _Element; }
        }

        public TeXAttributes(SerializationInfo info, StreamingContext context, TeXElement _Element)
            : base(info, context)
        {
            this._Element = _Element;
        }

        public TeXAttributes(IDictionary<string, string> dictionary, IEqualityComparer<string> comparer, TeXElement _Element)
            : base(dictionary, comparer)
        {
            this._Element = _Element;
        }

        public TeXAttributes(IDictionary<string, string> dictionary, TeXElement _Element)
            : base(dictionary)
        {
            this._Element = _Element;
        }

        public TeXAttributes(int capacity, IEqualityComparer<string> comparer, TeXElement _Element)
            : base(capacity, comparer)
        {
            this._Element = _Element;
        }

        public TeXAttributes(IEqualityComparer<string> comparer, TeXElement _Element)
            : base(comparer)
        {
            this._Element = _Element;
        }

        public TeXAttributes(int capacity, TeXElement _Element)
            : base(capacity)
        {
            this._Element = _Element;
        }

        public TeXAttributes(TeXElement _Element)
        {
            this._Element = _Element;
        }
    }

    public class TeXElement
    {
        protected TeXAttributes _Attributes;
        protected TeXElementCollection _InnerXml;
        protected TeXElement _Parent;
        protected string _TagName;
        protected string _InnerTeXt;
        protected bool _NeedTags = true;

        protected internal TeXElement() { }

        public TeXElement(string name, TeXElement parent)
        {
            _TagName = name;
            _Parent = parent;
            _Attributes = new TeXAttributes(this);
            _InnerXml = new TeXElementCollection(this);
        }

        public TeXElement(string name)
        {
            _TagName = name;
            _Attributes = new TeXAttributes(this);
            _InnerXml = new TeXElementCollection(this);
        }

        public string TagName
        {
            get { return _TagName; }
        }

        public TeXAttributes Attributes
        {
            get { return _Attributes; }
        }

        public TeXElementCollection InnerXml
        {
            get { return _InnerXml; }
        }

        public TeXElement Parent
        {
            get { return _Parent; }
            set { _Parent = value; }
        }

        public string InnerTeXt
        {
            get { return _InnerTeXt; }
            set { _InnerTeXt = value; }
        }

        public bool NeedTags
        {
            get { return _NeedTags; }
        }

        public bool HasContent
        {
            get { return !string.IsNullOrEmpty(InnerTeXt) || InnerXml.Count > 0; }
        }

        public TeXElement Ins(TeXElement elem)
        {
            InnerXml.Add(elem);
            elem.Parent = this;
            return this;
        }

        public TeXElement Con(TeXElement elem)
        {
            if (this is TeXMLDoc)
                throw new Exception("Cannot connect element to document root. Try to insert");
            if (Parent != null)
                Parent.Ins(elem);
            else
                throw new Exception("Cannot connect element. Parent is null");
            return elem;
        }

        protected static void RenderNewLineBlock(TextWriter writer)
        {
            writer.Write("\r\n");
        }

        protected virtual void RenderBeginTag(TextWriter writer)
        {
            string tagString = "";
            if (NeedTags)
            {
                tagString += "<" + TagName;
                foreach (KeyValuePair<string, string> attribute in _Attributes)
                {
                    tagString += " " + attribute.Key + "=\"" + attribute.Value + "\"";
                }
                if (HasContent)
                {
                    tagString += ">";
                } else
                    tagString += " />";
            }
            writer.Write(tagString);
        }

        protected virtual void RenderEndTag(TextWriter writer)
        {
            string tagString = "";
            if (NeedTags && HasContent)
                tagString += "</" + TagName + ">";
            writer.Write(tagString);
        }

        protected virtual void RenderContentXml(TextWriter writer)
        {
            foreach (TeXElement teXElement in _InnerXml)
            {
                teXElement.RenderElement(writer);
            }
        }

        protected virtual void RenderContentText(TextWriter writer)
        {
            writer.Write(InnerTeXt);
        }

        public virtual void RenderElement(TextWriter writer)
        {
            OnBeforeBeginTagRender(writer);
            RenderBeginTag(writer);
            OnAfterBeginTagRender(writer);
            OnBeforeContentXmlRender(writer);
            RenderContentXml(writer);
            OnAfterContentXmlRender(writer);
            OnBeforeContentTextRender(writer);
            RenderContentText(writer);
            OnAfterContentTextRender(writer);
            OnBeforeEndTagRender(writer);
            RenderEndTag(writer);
            OnAfterEndTagRender(writer);
        }

        protected virtual void OnAfterEndTagRender(TextWriter writer){}

        protected virtual void OnAfterContentTextRender(TextWriter writer){}

        protected virtual void OnBeforeEndTagRender(TextWriter writer){}

        protected virtual void OnBeforeContentTextRender(TextWriter writer){}

        protected virtual void OnAfterContentXmlRender(TextWriter writer){}

        protected virtual void OnBeforeContentXmlRender(TextWriter writer) { }

        protected virtual void OnAfterBeginTagRender(TextWriter writer) { }

        protected virtual void OnBeforeBeginTagRender(TextWriter writer) { }
    }

    public class TeXMLDoc : TeXElement
    {
        private readonly TeXRoot root;

        public TeXRoot Root
        {
            get { return root; }
        }

        public TeXMLDoc()
        {
            root = new TeXRoot("Root", this);
            root.Attributes.Add("xmlns", "http://getfo.sourceforge.net/texml/ns1");
        }

        public TeXMLDoc(string name)
        {
            _TagName = name;
            root = new TeXRoot("Root", this);
            root.Attributes.Add("xmlns", "http://getfo.sourceforge.net/texml/ns1");
        }

        public void WriteXml(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            //XmlDocument xDoc = new XmlDocument();
            //xDoc.RemoveAll();
            //XmlNode xRoot = xDoc.CreateElement(Root.TagName);
            //XmlAttributeCollection xColl = xRoot.Attributes;
            //foreach (KeyValuePair<string, string> attribute in Root.Attributes)
            //{
            //    XmlAttribute xValue = xDoc.CreateAttribute(attribute.Key);
            //    xValue.Value = attribute.Value;
            //    xColl.Append(xValue);
            //}
            //ParseTeXNode(Root, xRoot, xDoc);
            //xDoc.AppendChild(xRoot);
            //xDoc.Save(fs);
            StringWriter sw = new StringWriter();
            StreamWriter swr = new StreamWriter(fs, Encoding.UTF8);
            Root.RenderElement(sw);
            string s = sw.ToString();
            for (int i = 0; i < s.Length; i++)
            {
                swr.Write(s[i]);
            }
            swr.Flush();
            fs.Close();
        }

        protected static void ParseTeXNode(TeXElement xElem, XmlNode xRoot, XmlDocument xDoc)
        {
            if (string.IsNullOrEmpty(xElem.InnerTeXt))
                foreach (TeXElement xElement in xElem.InnerXml)
                {
                    if (xElement.NeedTags)
                    {
                        XmlNode xNode = xDoc.CreateElement(xElement.TagName);
                        XmlAttributeCollection xColl = xNode.Attributes;
                        foreach (KeyValuePair<string, string> attribute in xElement.Attributes)
                        {
                            XmlAttribute xValue = xDoc.CreateAttribute(attribute.Key);
                            xValue.Value = attribute.Value;
                            xColl.Append(xValue);
                        }
                        ParseTeXNode(xElement, xNode, xDoc);
                        if (xElem is TeXGroup)
                            xRoot.InnerXml += xNode.OuterXml;
                        else
                            xRoot.AppendChild(xNode);
                    } else
                        ParseTeXNode(xElement, xRoot, xDoc);
                } else
            {
                xRoot.InnerXml += xElem.InnerTeXt;
            }
        }
    }

    public class TeXRoot : TeXElement
    {
        private readonly string _InnerName;

        public string InnerName
        {
            get { return _InnerName; }
        }

        public TeXRoot()
            : base("TeXML")
        {
        }

        public TeXRoot(string name, TeXElement doc)
            : base("TeXML", doc)
        {
            _InnerName = name;
        }

        public TeXRoot PlainTeX()
        {
            return Escape(false).Ligatures(true).EmptyLines(true).Ws(true);
        }

        public TeXRoot Escape(bool needEsc)
        {
            string ne = needEsc ? "1" : "0";
            if (!Attributes.ContainsKey("escape"))
                Attributes.Add("escape", ne);
            else
                Attributes["escape"] = ne;
            return this;
        }

        public TeXRoot EmptyLines(bool needEmptyLines)
        {
            string ne = needEmptyLines ? "1" : "0";
            if (!Attributes.ContainsKey("emptylines"))
                Attributes.Add("emptylines", ne);
            else
                Attributes["emptylines"] = ne;
            return this;
        }

        public TeXRoot Ligatures(bool needLigatures)
        {
            string ne = needLigatures ? "1" : "0";
            if (!Attributes.ContainsKey("ligatures"))
                Attributes.Add("ligatures", ne);
            else
                Attributes["ligatures"] = ne;
            return this;
        }

        public TeXRoot Mode(TeXModes mode)
        {
            string ne = "text";
            switch (mode)
            {
                case TeXModes.Math:
                    ne = "math";
                    break;
                case TeXModes.Text:
                    ne = "text";
                    break;
            }
            if (!Attributes.ContainsKey("mode"))
                Attributes.Add("mode", ne);
            else
                Attributes["mode"] = ne;
            return this;
        }

        public TeXRoot Ws(bool needWs)
        {
            string ne = needWs ? "1" : "0";
            if (!Attributes.ContainsKey("ws"))
                Attributes.Add("ws", ne);
            else
                Attributes["ws"] = ne;
            return this;
        }

        protected override void OnBeforeBeginTagRender(TextWriter writer)
        {
            base.OnBeforeBeginTagRender(writer);
            if (HasContent)
                RenderNewLineBlock(writer);
        }

        protected override void OnAfterEndTagRender(TextWriter writer)
        {
            base.OnAfterEndTagRender(writer);
            RenderNewLineBlock(writer);
        }
    }

    public class TeXCmd : TeXElement
    {
        public TeXCmd(string cmdName, TeXElement parent)
            : base("cmd", parent)
        {
            Attributes.Add("name", cmdName);
        }

        public TeXCmd(string cmdName)
            : base("cmd")
        {
            Attributes.Add("name", cmdName);
        }

        public string CmdName
        {
            get { return Attributes["name"]; }
        }

        public TeXCmd NlBefore()
        {
            if (!Attributes.ContainsKey("nl1"))
                Attributes.Add("nl1", "1");
            else
                Attributes["nl1"] = "1";
            return this;
        }

        public TeXCmd NlAfter()
        {
            if (!Attributes.ContainsKey("nl2"))
                Attributes.Add("nl2", "1");
            else
                Attributes["nl2"] = "1";
            return this;
        }

        public TeXCmd Gr(bool needGroup)
        {
            string ng = needGroup ? "1" : "0";
            if (!Attributes.ContainsKey("gr"))
                Attributes.Add("gr", ng);
            else
                Attributes["gr"] = ng;
            return this;
        }

        /// <summary>
        /// Set parm for cmd. Delete all before
        /// </summary>
        /// <param name="par">parm in {}</param>
        /// <returns>this</returns>
        public TeXCmd Params(string par)
        {
            InnerXml.Add(new TeXElement("parm", this).Ins(new TeXText(par)));
            return this;
        }

        public TeXCmd Params(TeXElementCollection coll)
        {
            TeXElement parm = new TeXElement("parm", this);
            foreach (TeXElement xElement in coll)
                parm.Ins(xElement);
            return this;
        }

        /// <summary>
        /// Set opt for cmd. Deleta all before
        /// </summary>
        /// <param name="opt">opt in []</param>
        /// <returns>this</returns>
        public TeXCmd Opts(string opt)
        {
            InnerXml.Add(new TeXElement("opt", this).Ins(new TeXText(opt)));
            return this;
        }

        public TeXCmd Opts(TeXElementCollection coll)
        {
            TeXElement opt = new TeXElement("opt", this);
            foreach (TeXElement xElement in coll)
                opt.Ins(xElement);
            return this;
        }
    }

    public class TeXEnv : TeXElement
    {
        public TeXEnv(string name)
            : base("env")
        {
            _Attributes.Add("name", name);
        }

        public TeXEnv(string name, TeXElement parent)
            : base("env", parent)
        {
            _Attributes.Add("name", name);
        }

        protected override void OnBeforeBeginTagRender(TextWriter writer)
        {
            base.OnBeforeBeginTagRender(writer);
            if (HasContent)
                RenderNewLineBlock(writer);
        }

        protected override void OnAfterEndTagRender(TextWriter writer)
        {
            base.OnAfterEndTagRender(writer);
            RenderNewLineBlock(writer);
        }
    }

    public class TeXGroup : TeXElement
    {
        private readonly string _InnerName;

        public string InnerName
        {
            get { return _InnerName; }
        }

        public TeXGroup()
            : base("group")
        {
        }

        public TeXGroup(string name, TeXElement parent)
            : base("group", parent)
        {
            _InnerName = name;
        }
    }

    public class TeXMath : TeXElement
    {
        private readonly string _InnerName;

        public string InnerName
        {
            get { return _InnerName; }
        }

        public TeXMath()
            : base("math")
        {
        }

        public TeXMath(string name, TeXElement parent)
            : base("math", parent)
        {
            _InnerName = name;
        }
    }

    public class TeXDMath : TeXElement
    {
        private readonly string _InnerName;

        public string InnerName
        {
            get { return _InnerName; }
        }

        public TeXDMath()
            : base("dmath")
        {
        }

        public TeXDMath(string name, TeXElement parent)
            : base("dmath", parent)
        {
            _InnerName = name;
        }
    }

    public class TeXCtrl : TeXElement
    {
        public TeXCtrl(string ch)
            : base("ctrl")
        {
            _Attributes.Add("ch", ch);
        }

        public TeXCtrl(string ch, TeXElement parent)
            : base("ctrl", parent)
        {
            _Attributes.Add("ch", ch);
        }
    }

    public class TeXSpec : TeXElement
    {
        private readonly string[] specArray =
            {
                "esc",
                "bg",
                "eg",
                "mshift",
                "align",
                "parm",
                "sup",
                "sub",
                "tilde",
                "comment",
                "vert",
                "lt",
                "gt",
                "nl",
                "nl?",
                "space",
                "nil"
            };

        public enum SpecSimbols
        {
            /// <summary>
            /// Print "\" to tex
            /// </summary>
            Esc = 0,
            /// <summary>
            /// Print "{" to tex
            /// </summary>
            Bg,
            /// <summary>
            /// Print "}" to tex
            /// </summary>
            Eg,
            /// <summary>
            /// Print "$" to tex
            /// </summary>
            Mshift,
            /// <summary>
            /// Print "&" to tex
            /// </summary>
            Align,
            /// <summary>
            /// Print "#" to tex
            /// </summary>
            Parm,
            /// <summary>
            /// Print "^" to tex
            /// </summary>
            Sup,
            /// <summary>
            /// Print "_" to tex
            /// </summary>
            Sub,
            /// <summary>
            /// Print "~" to tex
            /// </summary>
            Tilde,
            /// <summary>
            /// Print "%" to tex
            /// </summary>
            Comment,
            /// <summary>
            /// Print "|" to tex
            /// </summary>
            Vert,
            /// <summary>
            /// Print "&lt;" to tex
            /// </summary>
            Lt,
            /// <summary>
            /// Print "&gt;" to tex
            /// </summary>
            Gt,
            /// <summary>
            /// Print new line to tex
            /// </summary>
            Nl,
            /// <summary>
            /// Print new line conditional to tex
            /// </summary>
            Nln,
            /// <summary>
            /// Print simple space to tex
            /// </summary>
            Space,
            /// <summary>
            /// Print nothing to tex
            /// </summary>
            Nil
        }

        public TeXSpec(SpecSimbols simbol)
            : base("spec")
        {
            Attributes.Add("cat", specArray[(int)simbol]);
        }

        public TeXSpec(string cat)
            : base("spec")
        {
            Attributes.Add("cat", cat);
        }

        public TeXSpec(string cat, TeXElement parent)
            : base("spec", parent)
        {
            Attributes.Add("cat", cat);
        }
    }

    public class TeXPdf : TeXElement
    {
        private readonly string _InnerName;

        public string InnerName
        {
            get { return _InnerName; }
        }

        public TeXPdf()
            : base("pdf")
        {
            _InnerName = "";
        }

        public TeXPdf(string name, TeXElement parent)
            : base("pdf", parent)
        {
            _InnerName = name;
        }
    }

    public class TeXText : TeXElement
    {
        public TeXText(string text)
            : base("text")
        {
            _NeedTags = false;
            _InnerTeXt = text;
        }

        public TeXText(string text, TeXElement parent)
            : base("text", parent)
        {
            _NeedTags = false;
            _InnerTeXt = text;
        }
    }
}
