﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace STCT.DBInstall
{
    public class NativeMethods
    {
        [DllImport("msi.dll", SetLastError = true)]
        static extern uint MsiOpenDatabase(string szDatabasePath, IntPtr phPersist, out IntPtr phDatabase);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        static extern int MsiDatabaseOpenViewW(IntPtr hDatabase, [MarshalAs(UnmanagedType.LPWStr)] string szQuery, out IntPtr phView);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        static extern int MsiViewExecute(IntPtr hView, IntPtr hRecord);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        static extern uint MsiViewFetch(IntPtr hView, out IntPtr hRecord);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        static extern int MsiRecordGetString(IntPtr hRecord, int iField,
           [Out]  StringBuilder szValueBuf, ref int pcchValueBuf);

        [DllImport("msi.dll", ExactSpelling = true)]
        static extern IntPtr MsiCreateRecord(uint cParams);

        [DllImport("msi.dll", ExactSpelling = true)]
        static extern uint MsiCloseHandle(IntPtr hAny);

        public const string Version = "ProductVersion";
        public const string ProductCode = "ProductCode";
        public const string ProductName = "ProductName";

        public static string GetVersionInfo(string fileName, string property)
        {
            try
            {
                string sqlStatement = string.Format("SELECT * FROM Property WHERE Property = '{0}'", property);
                IntPtr phDatabase = IntPtr.Zero;
                IntPtr phView = IntPtr.Zero;
                IntPtr hRecord = IntPtr.Zero;

                StringBuilder szValueBuf = new StringBuilder();
                int pcchValueBuf = 255;

                // Open the MSI database in the input file 
                uint val = MsiOpenDatabase(fileName, IntPtr.Zero, out phDatabase);

                hRecord = MsiCreateRecord(1);

                // Open a view on the Property table for the version property 
                int viewVal = MsiDatabaseOpenViewW(phDatabase, sqlStatement, out phView);

                // Execute the view query 
                int exeVal = MsiViewExecute(phView, hRecord);

                // Get the record from the view 
                uint fetchVal = MsiViewFetch(phView, out hRecord);

                // Get the version from the data 
                int retVal = MsiRecordGetString(hRecord, 2, szValueBuf, ref pcchValueBuf);

                MsiCloseHandle(phDatabase);
                MsiCloseHandle(phView);
                MsiCloseHandle(hRecord);

                return szValueBuf.ToString();
            }
            catch(AccessViolationException ex)
            {
                return "";
            }
            catch(Exception ex)
            {
                return "";
            }
        }
    }
}
