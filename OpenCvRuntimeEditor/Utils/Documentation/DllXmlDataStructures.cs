namespace OpenCvRuntimeEditor.Utils.Documentation
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "doc")]
    public class XML_Doc
    {
        [XmlElement(ElementName = "members")] public Doc_Members Members = new Doc_Members();
    }

    [XmlRoot(ElementName = "members")]
    public class Doc_Members
    {
        [XmlElement(ElementName = "assembly")] public Doc_Assembly Assembly = new Doc_Assembly();
        [XmlElement(ElementName = "member")] public List<Doc_Node> NodesDocs = new List<Doc_Node>();
    }

    [XmlRoot(ElementName = "assembly")]
    public class Doc_Assembly
    {
        [XmlElement(ElementName = "name")] public string Name = "";
    }

    [XmlRoot(ElementName = "member")]
    public class Doc_Node
    {
        [XmlAttribute(AttributeName = "name")] public string Name = "";
        [XmlElement(ElementName = "summary")] public Doc_Summary Summary = new Doc_Summary();
        [XmlElement(ElementName = "param")] public List<Doc_Param> Params = new List<Doc_Param>();
        [XmlElement(ElementName = "returns")] public Doc_Returns Returns = new Doc_Returns();
    }

    [XmlRoot(ElementName = "summary")]
    public class Doc_Summary
    {
        [XmlElement(ElementName = "para")] public string Para = "";
        [XmlText] public string Info = "";
    }

    [XmlRoot(ElementName = "param")]
    public class Doc_Param
    {
        [XmlAttribute(AttributeName = "name")] public string Name = "";
        [XmlText] public string Text = "";
    }

    [XmlRoot(ElementName = "returns")]
    public class Doc_Returns
    {
        [XmlElement(ElementName = "para")] public string Para = "";
        [XmlText] public string Info = "";
    }
}
