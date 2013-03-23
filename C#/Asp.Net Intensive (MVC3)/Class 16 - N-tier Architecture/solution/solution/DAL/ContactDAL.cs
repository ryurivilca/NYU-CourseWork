﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.IO;
using System.Text;

namespace ContactWeb.Models
{
    public class ContactDAL : BUS.IContactDAL
    {

        public ContactDAL()
        {
            if (!File.Exists(this.GetPath()))
                Populate();
        }

        private Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden

        
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 *random.NextDouble() + 65)));
                builder.Append(i == 0 ? ch : char.ToLower(ch));
            }

            return builder.ToString();
        }
         
        private void Populate()
        {
            var doc = new XDocument(
                    new XElement("Contacts")
                );
            var root = doc.Element("Contacts");
            var contacts = new List<Contact>();
                    for (var i = 1; i <= 500; i++)
                    {
                        var firstName = RandomString(4);
                        var lastName = RandomString(6);
                        string email = String.Format("{0}_{1}@gmail.com", firstName, lastName);

                        var counter = 0;
                        while (contacts.Count(c => c.Email == email) > 0)
                        {
                            counter++;
                            email = String.Format("{0}_{1}{2}@gmail.com", firstName, lastName, counter);
                        }

                        contacts.Add(
                            new Contact
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                Id = i,
                                Email = email
                            }
                        );
                    }

            foreach(var contact in contacts)
                root.Add(ToXElement(contact));
            doc.Save(GetPath());

        }

        private Contact FromXElement(XElement node)
        {
            return new Contact
            {
                Id = (int)node.Element("Id"),
                FirstName = node.Element("FirstName").Value,
                LastName = node.Element("LastName").Value,
                Email = node.Element("Email").Value,
                LuckyNumber =node.Element("LuckyNumber") != null ? (int)node.Element("LuckyNumber") : (int?)null
            };
        }

        private XElement ToXElement(Contact contact)
        {
            var node =  new XElement("Contact",
                        new XElement("Id", contact.Id),
                        new XElement("FirstName", contact.FirstName),
                        new XElement("LastName", contact.LastName),
                        new XElement("Email", contact.Email)
                );

            if (contact.LuckyNumber.HasValue)
                node.Add(new XElement("LuckyNumber", contact.LuckyNumber.Value));

            return node;
        }

        private string GetPath()
        {
            return System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Contacts.xml");
        }

        private XDocument GetDocument()
        {
            return XDocument.Load(GetPath());
        }

        public List<Contact> GetContacts()
        {
            var doc = GetDocument();

            var results = from contact in doc.Descendants("Contact")
                          select FromXElement(contact);

            return results.ToList();
        }

        public Contact GetContact(int id)
        {
            var doc = GetDocument();
            var results = doc.Descendants("Contact").Where(contact => (int)contact.Element("Id") == id).Select(contact => FromXElement(contact));
            return results.SingleOrDefault();
        }

        public Contact GetContact(string email)
        {
            var doc = GetDocument();
            var results = doc.Descendants("Contact").Where(contact => contact.Element("Email").Value.ToLower() == email.ToLower()).Select(contact => FromXElement(contact));
            return results.SingleOrDefault();
        }

        public int Create(Contact contact)
        {
            var id = GetContacts().Count > 0 ?  this.GetContacts().Max(c => c.Id) + 1 : 1;
            contact.Id = id;
            var doc = this.GetDocument();
            doc.Element("Contacts").Add(ToXElement(contact));
            doc.Save(GetPath());

            return id;
        }

        public void Edit(Contact contact)
        {
            var doc = GetDocument();
            var nodeToEdit = doc.Descendants("Contact").Where(node => (int)node.Element("Id") == contact.Id).SingleOrDefault();
            nodeToEdit.ReplaceWith(ToXElement(contact));
            doc.Save(GetPath());
        }

        public void Delete(Contact contact)
        {
            var doc = GetDocument();
            var nodeToDelete = doc.Descendants("Contact").Where(node => (int)node.Element("Id") == contact.Id).SingleOrDefault();
            nodeToDelete.Remove();
            doc.Save(GetPath());
        }

    }
}