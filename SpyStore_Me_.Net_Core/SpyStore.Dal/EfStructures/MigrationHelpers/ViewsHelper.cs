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
    public static class ViewsHelper
    {
        public static void CreateOrderDetailWithProductInfoView(MigrationBuilder migrationBuilder)
        {
            string createView = @"
                        CREATE VIEW [Store].[OrderDetailWithProductInfo] AS
                            SELECT od.Id, od.TimeStamp, od.OrderId, od.ProductId,
                                    od.Quantity, od.UnitCost,
                                    od.Quantity * od.UnitCost AS LineItemTotal,
                                    p.ModelName, p.Description, p.ModelNumber, p.ProductImage,
                                    p.ProductImageLarge, p.ProductImageThumb, p.CategoryId,
                                    p.UnitsInStock, p.CurrentPrice, c.CategoryName
                            FROM Store.OrderDetails od
                                INNER JOIN Store.Orders o ON o.Id = od.OrderId
                                INNER JOIN Store.Products p ON p.Id = od.ProductId
                                INNER JOIN Store.Categories c ON c.Id = p.CategoryId";
            migrationBuilder.Sql(createView);
        }
        public static void CreateCartRecordWithProductInfoView(MigrationBuilder migrationBuilder)
        {
            string createView = @"
                        CREATE VIEW [Store].[CartRecordWithProductInfo] AS
                            SELECT scr.Id, scr.TimeStamp, scr.DateCreated, scr.CustomerId,
                                    scr.Quantity, scr.LineItemTotal, scr.ProductId, p.ModelName,
                                    p.Description, p.ModelNumber, p.ProductImage,
                                    p.ProductImageLarge, p.ProductImageThumb, p.CategoryId,
                                    p.UnitsInStock, p.CurrentPrice, c.CategoryName
                            FROM ShoppingCartRecords scr
                                INNER JOIN Products p ON p.Id = scr.ProductId
                                INNER JOIN Categories c ON c.Id = p.CategoryId";
            migrationBuilder.Sql(createView);
        }

        public static void DropOrderDetailWithProductInfoView(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [Store].[OrderDetailWithProductInfo]");
        }

        public static void DropCartRecordWithProductInfoView(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [Store].[CartRecordWithProductInfo]");
        }
    }
}
