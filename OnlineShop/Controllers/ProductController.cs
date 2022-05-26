using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyNet5ApiAdoTest.Services;
using OnlineShop.Models;
using OnlineShop.Tool;
using System;
using System.Collections.Generic;
using System.Data;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        //SQL連線字串 SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

        //建立DataTable表單
        private static DataTable CarDatatable()
        {
            DataTable dt = new DataTable();
            DataColumn[] dtc = new DataColumn[1];
            dtc[0] = new DataColumn("f_id", System.Type.GetType("System.Int32"));
            dt.Columns.AddRange(dtc);
            return dt;
        }

        //商品相關----------------------------
        //取得產品資料
        [HttpGet("GetProduct")]
        public string GetProduct()
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @"EXEC pro_onlineShop_getProduct";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                //關閉連線
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }

            //DataTable轉Json;
            var result = Tool.InTool.DataTableJson(dt);

            return result;
        }

        //商品類別相關------------------------
        //取得類別
        [HttpGet("GetCategory")]
        public string GetCategory()
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @" EXEC pro_onlineShop_getProductCategory ";

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);

                da.Fill(ds);


            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                //關閉連線
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }
            //DataTable轉Json;
            var result = InTool.DataTableJson(dt);

            return result;
        }

        //購物車相關------------------------
        //取得Cookie組購物車
        [HttpGet("GetCar")]
        public string GetCar([FromQuery] int id)
        {
            //cookie 帶條件回來
            //SQL 進庫依照條件找資料
            var carItem = "";
            string[] CarID = new string[0];
            if (Request.Cookies.ContainsKey("carItem"))
            {
                carItem = Request.Cookies["carItem"];
                CarID = carItem.Split("/");
            }

            DataTable Cardt = CarDatatable();

            //CarID判斷有幾個數
            for (var i = 0; i < CarID.Length; i++)
            {
                DataRow dr = Cardt.NewRow();
                dr[0] = CarID[i];
                Cardt.Rows.Add(dr);
            }

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //cmd.CommandText = @"EXEC pro_onlineShop_getProductCar @ProductCar ";
                cmd.CommandText = @"[dbo].[pro_onlineShop_getProductCar]";
                cmd.Parameters.AddWithValue("@ProductCar", Cardt);

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                //關閉連線
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }

            //DataTable轉Json;
            var result = Tool.InTool.DataTableJson(dt);

            return result;
        }
    }
}
