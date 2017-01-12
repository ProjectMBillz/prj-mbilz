<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewData.aspx.cs" Inherits="ViewData" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>View Items</title>
</head>
<body>
    <form id="ListForm" runat="server">
    <div>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" AutoGenerateDeleteButton="True" AutoGenerateEditButton="True" AutoGenerateSelectButton="false" Caption="Item List" DataSourceID="ItemList" DataKeyNames="Id">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" />
                <asp:BoundField DataField="Price" HeaderText="Price" SortExpression="Price" />
                <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
            </Columns>
        </asp:GridView>
        <asp:HyperLink ID="InsertItem" runat="server" NavigateUrl="~/InsertItem.aspx">Add New Item</asp:HyperLink>
        <asp:ObjectDataSource ID="ItemList" runat="server"
            SelectMethod="GetAllItems" TypeName="NHibernate.Example.Web.Persistence.ItemList" DataObjectTypeName="NHibernate.Example.Web.Domain.Item" DeleteMethod="DeleteItem" InsertMethod="InsertItem" UpdateMethod="UpdateItem">
            <DeleteParameters>
                <asp:Parameter Name="id" Type="Int32" />
            </DeleteParameters>
        </asp:ObjectDataSource>
    </div>
    </form>
</body>
</html>
