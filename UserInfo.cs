using System;
using System.Collections.Generic;

public class UserModel
{
	public string email;
	public string name;
	public string phone;
	public List<string> messages;
    public int age;
	public UserModel() { }
	public UserModel(string email,
	string name,
	string phone,List<string> messages,int age) { this.email = email; this.name = name; this.phone = phone; this.messages = messages; this.age = age; }


}
