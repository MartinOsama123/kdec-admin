using System;

public class UserModel
{
	string email;
	string name;
	string phone;
	
	public UserModel() { }
	public UserModel(string email,
	string name,
	string phone) { this.email = email; this.name = name; this.phone = phone; }
}
