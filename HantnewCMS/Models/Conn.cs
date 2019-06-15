using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace HantnewCMS.Models
{
    public class Conn
    {
        protected SqlConnection conn;

        public void OpenConnection()
        {
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
            try
            {
                if (conn.State.ToString() != "Open")
                {
                    conn.Open();
                }
            }
            catch (SqlException ex)
            {

                throw ex;
            }
        }

        public void CloseConnection()
        {
            try
            {
                conn.Close();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }
        public int Drop(string sql)
        {
            int i = 0;
            try
            {
                if (conn.State.ToString() == "Open")
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Create

        public int CreateTable(string sql)
        {
            int i = 0;
            try
            {
                if (conn.State.ToString() == "Open")
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Insert
        public int InsertData(string sql)
        {
            int i = 0;
            try
            {
                if (conn.State.ToString() == "Open")
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update
        public int UpdateData(string sql)
        {
            int i = 0;
            try
            {
                if (conn.State.ToString() == "Open")
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int Select(string sql)
        {
            try
            {
                SqlCommand com = new SqlCommand(sql, conn);
                int count = (int)(com.ExecuteScalar());
                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable List(string sql)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet Ds_List(string sql)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //Detail
        public DataTable Detail(string sql)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete
        public int Delete(string sql)
        {
            try
            {
                int result = 0;
                SqlCommand cmd = new SqlCommand(sql, conn);
                result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}