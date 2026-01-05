using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DR.Models
{   
	[XmlRoot(ElementName="number")]
	public class Number {
		[XmlAttribute(AttributeName="type")]
		public string Type { get; set; }
		[XmlText]
		public string Text { get; set; }
	}


	[XmlRoot(ElementName="address")]
	public class Address {
     
		[XmlElement(ElementName="number")]
		public Number Number { get; set; }
	}


	[XmlRoot(ElementName="destination")]
	public class Destination {
       
		[XmlElement(ElementName="address")]
		public Address Address { get; set; }
	}


	[XmlRoot(ElementName="source")]
	public class Source {
    
		[XmlElement(ElementName="address")]
		public Address Address { get; set; }
	}


	[XmlRoot(ElementName="rsr_detail")]
	public class Rsr_detail {
     
		[XmlElement(ElementName="description")]
		public string Description { get; set; }
       
		[XmlElement(ElementName="code")]
		public string Code { get; set; }
		[XmlAttribute(AttributeName="status")]
		public string Status { get; set; }
	}


	[XmlRoot(ElementName="rsr")]
	public class Rsr {

		[XmlElement(ElementName="service-id")]
		public string Serviceid { get; set; }

		[XmlElement(ElementName="destination")]
		public Destination Destination { get; set; }

		[XmlElement(ElementName="source")]
		public Source Source { get; set; }

		[XmlElement(ElementName="rsr_detail")]
		public Rsr_detail Rsr_detail { get; set; }
		[XmlAttribute(AttributeName="type")]
		public string Type { get; set; }
	}

	[DataContract]
	[XmlRoot(ElementName="message")]
	public class Message {
		[DataMember]
		[XmlElement(ElementName="rsr")]
		public Rsr Rsr { get; set; }
		[XmlAttribute(AttributeName="id")]
		public string Id { get; set; }
	}

}
