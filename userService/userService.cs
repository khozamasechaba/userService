using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IUserService
{
    bool addToDb (string username, string name, string surname);
    void showDB();
    bool checkExists(string username);

}
public class UserService: IUserService
{
    private IDatabaseAccess _databaseAccess;
    public UserService(IDatabaseAccess databaseAccess)
    { 
        _databaseAccess = databaseAccess;
    }


    public bool addToDb(string username, string name, string surname)
        
    {
        bool exists = false;
        
        if (checkExists(username) == false)
        {
            _databaseAccess.InsertToDB(username, name, surname);
            //exists = false;
            
        }
        else
        {
            
            int requests = 1;
           _databaseAccess.InsertToHistory(username, requests);
           Console.WriteLine("This user already exists!");
           Console.WriteLine(" ");
            exists = true;

        }
        return exists;
    }

    public  void showDB() {

        _databaseAccess.ReadFromDB();
    }

    public  bool checkExists(string username)
        {
            if(_databaseAccess.CheckUserExists(username) == true)
                {
                    return true;
                }
            return false;
        }
}   

    
