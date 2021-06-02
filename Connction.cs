﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;

namespace AddressBookADonet
{
    class AddressBookRepo
    {
        public static string connectionString = "Data Source =.; Initial Catalog = AddressBook; Integrated Security = True";
        SqlConnection connection = new SqlConnection(connectionString);

        public void CheckConnection()
        {
            try
            {
                using (this.connection)
                {
                    connection.Open();
                    Console.WriteLine("Database Connection OK");
                }
            }
            catch
            {
                Console.WriteLine("Database not connected");
            }
            finally
            {
                this.connection.Close();
            }
        }
        

        public void GetAllContact()
        {
            ///Creates a new connection for every method to avoid "ConnectionString property not initialized" exception
            DBConnection dbc = new DBConnection();
            connection = dbc.GetConnection();
            AddressBookModel model = new AddressBookModel();
            try
            {
                using (connection)
                {
                    /// Query to get all the data from the table
                    string query = @"select * from dbo.AddressBookProfiles";
                    
                    SqlCommand command = new SqlCommand(query, connection);
                    ///Opening the connection.
                    connection.Open();
                    /// executing the sql data reader to fetch the records
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        /// Mapping the data to the employee model class object
                        while (reader.Read())
                        {
                            model.FirstName = reader.GetString(0);
                            model.LastName = reader.GetString(1);
                            model.Address = reader.GetString(2);
                            model.City = reader.GetString(3);
                            model.State = reader.GetString(4);
                            model.Zip = reader.GetInt32(5);
                            model.PhoneNumber = reader.GetInt32(6);
                            model.EmailId = reader.GetString(7);

                            Console.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", model.FirstName, model.LastName,
                                model.Address, model.City, model.State, model.Zip, model.PhoneNumber, model.EmailId, model.AddressBookType, model.AddressBookName);
                            Console.WriteLine("\n");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No data found");
                    }
                    reader.Close();
                }
            }
            /// Catching the null record exception
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            /// Always ensuring the closing of the connection
            finally
            {
                connection.Close();
            }

        }
        public class DBConnection
        {

            public SqlConnection GetConnection()
            {
                /// Specifying the connectionString from the sql server connection.
                string connectiongString = @"Data Source =.; Initial Catalog = AddressBook; Integrated Security = True";
                SqlConnection connection = new SqlConnection(connectiongString);
                return connection;
            }
        }
        public bool AddContact(AddressBookModel model)
        {
            try
            {
                using (this.connection)
                {
                    SqlCommand command = new SqlCommand("SpAddContactInAddressBook", this.connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@first_name", model.FirstName);
                    command.Parameters.AddWithValue("@last_name", model.LastName);
                    command.Parameters.AddWithValue("@address", model.Address);
                    command.Parameters.AddWithValue("@city", model.City);
                    command.Parameters.AddWithValue("@state", model.State);
                    command.Parameters.AddWithValue("@zip", model.Zip);
                    command.Parameters.AddWithValue("@phone_number", model.PhoneNumber);
                    command.Parameters.AddWithValue("@email", model.EmailId);
                    command.Parameters.AddWithValue("@addressbook_name", model.AddressBookName);
                    command.Parameters.AddWithValue("@addressbook_type", model.AddressBookType);
                    this.connection.Open();
                    var result = command.ExecuteNonQuery();
                    this.connection.Close();
                    if (result != 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                this.connection.Close();
            }
        }
    }


}
