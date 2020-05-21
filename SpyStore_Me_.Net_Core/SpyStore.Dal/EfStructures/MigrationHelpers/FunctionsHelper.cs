using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;

//These classes will contain the code that is called by the Up and Down methods 
//of the TSQL migration. 
//Instead of directly adding code to the Up and Down methods, placing them in helper 
//classes will prevent losing work if the migration needs to be removed and recreated. 
namespace SpyStore.Dal.EfStructures.MigrationHelpers
{
    public static class FunctionsHelper
    {
        public static void CreateOrderTotalFunction(MigrationBuilder migrationBuilder)
        {
            string createFunction = @"CREATE FUNCTION Store.GetOrderTotal(@OrderId INT)
                            RETURNS MONEY WITH SCHEMABINDING
                            BEGIN
                                DECLARE @Result MONEY
                                SELECT @Result = SUM([Quantity]*[UnitCost]) 
                                FROM Store.OrderDetails
                                WHERE OrderId = @OrderId;
                            RETURN coalesce(@Result,0);
                            END";
            migrationBuilder.Sql(createFunction);
        }

        public static void DropOrderTotalFunction(MigrationBuilder builder)
        {
            builder.Sql("Drop function [Store].[GetOrderTotal]");
        }
    }
}
