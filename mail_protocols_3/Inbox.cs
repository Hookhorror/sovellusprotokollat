using System;
using System.Text;
using System.Collections.Generic;

class Inbox
{
    List<Email> emails;

    public Inbox()
    {
        emails = new List<Email>();
    }

    public void Add(Email email)
    {
        emails.Add(email);
    }

    public Email Retr(int emailNumber)
    {
        return emails[emailNumber - 1];
    }

    public int GetCount()
    {
        return emails.Count;
    }
}