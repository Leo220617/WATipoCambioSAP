﻿using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 

namespace WATickets.Controllers
{
    public class Conexion
    {
        public readonly static Conexion _instance = new Conexion();
        public static Company _company = null;

        G G = new G();

        private static Conexion Instance
        {
            get
            {
                return _instance;
            }
        }

        public static Company Company
        {
            get
            {

                if (_company == null)
                    new Conexion().DoSapConnection();



                var ins = Instance;
                return _company;
            }
        }

        public int DoSapConnection()
        {
            try
            {

                _company = new Company
                {
                    Server = G.ObtenerConfig("ServerSQL"),
                    LicenseServer = G.ObtenerConfig("ServerLicense"),
                    DbServerType = getBDType(G.ObtenerConfig("SQLType")),
                    language = BoSuppLangs.ln_English,
                    CompanyDB = G.ObtenerConfig("SQLBD"),
                    UserName = G.ObtenerConfig("SAPUser"),
                    Password = G.ObtenerConfig("SAPPass")
                };

                var resp = _company.Connect();




                if (resp != 0)
                {
                    var msg = _company.GetLastErrorDescription();
                    G.GuardarTxt("ErrorSAP.txt", msg);
                    return -1;
                }

                return resp;
            }
            catch (Exception ex)
            {

                G.GuardarTxt("ErrorSAP.txt", ex.Message);

                return -1;
            }

        }


        private BoDataServerTypes getBDType(string sql)
        {
            switch (sql)
            {
                case "2005":
                    return BoDataServerTypes.dst_MSSQL2005;
                case "2008":
                    return BoDataServerTypes.dst_MSSQL2008;
                case "2012":
                    return BoDataServerTypes.dst_MSSQL2012;
                case "2014":
                    return BoDataServerTypes.dst_MSSQL2014;
                case "2016":
                    return BoDataServerTypes.dst_MSSQL2016;
                case "2017":
                    return BoDataServerTypes.dst_MSSQL2017;
                case "2019":
                    return BoDataServerTypes.dst_MSSQL2019;
                case "HANA":
                    return BoDataServerTypes.dst_HANADB;
                default:
                    return BoDataServerTypes.dst_MSSQL;
            }
        }

        public static bool Desconectar()
        {
            try
            {
                if (_company != null && _company.Connected)
                {
                    _company.Disconnect();
                    _company = null;
                }
                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }
    }
}