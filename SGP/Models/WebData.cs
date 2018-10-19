using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGP.Models
{
    public class WebData
    {
    }
    public class ZoneList
    {
        public string ZoneID { get; set; }
    }
    public class getPostOffice
    {
        public string PostOfficeID { get; set; }
        public string PostOfficeName { get; set; }
    }
    public class ParaMailers
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
    public class ResponeMailers
    {
        public string PostOfficeID { get; set; }
        public int? TongCG { get; set; }
        public int? TongSL { get; set; }
        public double? TongTL {get;set;}
    }
    public class ResponseAmountByMonth
    {
        public int? Thang { get; set; }
        public decimal? DoanhThu {get;set;}
    }
    public class ResponseServiceMonth
    {
        public string DV { get; set; }
        public Nullable<int> SL { get; set; }
        public Nullable<double> TL { get; set; }
        public Nullable<decimal> DoanhThu { get; set; }
        public Nullable<decimal> PhanTram { get; set; }
    }
    public class ResponsePostOffice
    {
        public string MBC { get; set; }
        public string BC { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
    }

    public class PostOffice
    {
        public string ZoneID { get; set; }
        public string PostOfficeName { get; set; }
        public int TongSoPhat { get; set; }
        public int DaPhat { get; set; }
        public string TiLe { get; set; }
    }
    public class ParaLogin
    {
        public string User { get; set; }
        public string Pass { get; set; }
        public string Post { get; set; }
    }
    public class ResponeLogin
    {
        public string User { get; set; }
        public int Success { get; set; }
    }
    public class ResponePostOfficeAmount
    {
        public int STT { get; set; }
        public string PostOfficeID { get; set; }
        public string PostOfficeName { get; set; }
        public int TotalMailer { get; set; }
        public int TotalQuantity { get; set; }
        public double TotalWeight { get; set; }
        public decimal BefVATAmount { get; set; }
        public decimal Amount { get; set; }
        public string MailerDescription { get; set; }
    }
    public class ResponeMailerDeliveryMaster2kg8kg
    {
        public string BC { get; set; }
        public int Duoi_2kg_CG { get; set; }
        public int Duoi_2kg_SL { get; set; }
        public double Duoi_2kg_TL { get; set; }
        public double Duoi_2kg_TLK { get; set; }
        public int Tren_2kg_CG { get; set; }
        public int Tren_2kg_SL { get; set; }
        public double Tren_2kg_TL { get; set; }
        public double Tren_2kg_TLK { get; set; }
        public int Tren_8kg_CG { get; set; }
        public int Tren_8kg_SL { get; set; }
        public double Tren_8kg_TL { get; set; }
        public double Tren_8kg_TLK { get; set; }
    }
    public class ResponeMailerDeliveryDetail2kg8kg
    {
        public string BC { get; set; }

        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int Duoi_2kg_SL { get; set; }
        public double Duoi_2kg_TL { get; set; }
        public int Tren_2kg_SL { get; set; }
        public double Tren_2kg_TL { get; set; }
        public int Tren_8kg_SL { get; set; }
        public double Tren_8kg_TL { get; set; }
    }
    public class ResponseMailerByDate
    {
        public DateTime? AcceptDate { get; set; }
        public string MailerID { get; set; }
        public string SenderID { get; set; }
        public string SenderName { get; set; }
        public string SenderProvinceID { get; set; }
        public string ReceiveProvinceID { get; set; }
        public string RecieverDistrictID { get; set; }
        public string ServiceTypeID { get; set; }
        public string MailerTypeID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<double> RealWeight { get; set; }
        public Nullable<double> Weight { get; set; }
        public Nullable<decimal> Money { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> PriceDefault { get; set; }
        public Nullable<decimal> PriceService { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<decimal> BefVATAmount { get; set; }
        public Nullable<double> VATPercent { get; set; }
        public Nullable<decimal> VATAmount { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> AmountBefDiscount { get; set; }
        public string PostOfficeAcceptID { get; set; }
        public string PaymentMethodID { get; set; }
        public string PostOfficeRecieverMoneyID { get; set; }
        public string MailerDescription { get; set; }
        public string ThirdpartyDocID { get; set; }
        public Nullable<decimal> ThirdpartyCost { get; set; }
        public Nullable<decimal> CommissionAmt { get; set; }
        public Nullable<double> CommissionPercent { get; set; }
        public Nullable<decimal> CostAmt { get; set; }
        public DateTime? SalesClosingDate { get; set; }
        public string RecieverProvinceID { get; set; }
        public Nullable<double> DiscountPercent { get; set; }
        public string PostOfficeID { get; set; }
        public string PostOfficeName { get; set; }
        public string ZoneID { get; set; }

    }
    public  class ResponseIndex
    {
        public Nullable<decimal> DoanhThu { get; set; }
        public int PhieuGui { get; set; }
        public int SoLuong { get; set; }
        public double TrongLuong { get; set; }
        public int NgoaiTuyen { get; set; }
    }
    public class ResponseDelivery
    {
        public string BC { get; set; }
        public int DaNhan {get;set;}
        public int DaPhat { get; set; }
        public int ChuyenHoan { get; set; }
        public int Khac { get; set; }
        public int ChuaPhat { get; set; }
    }
    public class ResponseNotDeliveryDetail
    {
        public string EmployeeID {get;set;}	
        public string EmployeeName	{get;set;}
        public string MailerID	{get;set;}
        public DateTime  AcceptDate	{get;set;}
        public double   Weight {get;set;}
        public string   PostOfficeName	{get;set;}
        public string  StatusID	{get;set;}
        public string   StatusName	{get;set;}
        public string   ServiceTypeName {get;set;}	
        public string   MailerTypeName	{get;set;}
        public string   CurrentPostOffice {get;set;}
        public string ThoiGian { get; set; }

    }
    
    public class ResponeNotMailerDetail
    {
        public string PostOfficeName { get; set; }
        public string MailerID { get; set; }
        public string PostOfficeAcceptID { get; set; }
        public string ServiceTypeID { get; set; }
        public int ChenhLech { get; set; }
    }
    public class ResponseDeliveryTime
    {
        public string DocumentID { get; set; }
        public DateTime DocumentTime { get; set; }
        public string PostOfficeID { get; set; }
        public int Quantity { get; set; }
        public double Weight { get; set; }
        public string EmployeeID { get; set; }
        public string Time { get; set; }
    }
    public class ResponeDeliveryEmployeeZone1
    {
        public string BCGoc { get; set; }
        public string MailerID { get; set; }
        public string ChuyenThu { get; set; }
        public int Quantity { get; set; }
        public double Weight { get; set; }
        public double RealWeight { get; set; }
        public string ServiceTypeID { get; set; }
        public string PostOfficeID { get; set; }
        public string EmployeeID { get; set; }
        public string NgayGui { get; set; }
        public string DeliveryTo { get; set; }
        public string NgayNhan { get; set; }
        public string GioNhan { get; set; }
        public string MailerTypeID { get; set; }
        public string MailerDescription { get; set; }

    }
    public class ResponeTongCG
    {
        public string ZoneID { get; set; }
        public string MaBC { get; set; }
        public string BC { get; set; }
        public int TongCG { get; set; }
        public int ChuaNhapDT { get; set; }
    }
    public class ResponeTongCG_BC
    {
        public string MailerID { get; set; }
        public DateTime AcceptDate { get; set; }
        public string SenderID { get; set; }
        public string SenderName { get; set; }
        public int Quantity { get; set; }
        public double Weight { get; set; }
        public string ReceiveProvinceID { get; set; }
        public string ServiceTypeID { get; set; }
        public decimal Price { get; set; }
        public string MailerDescription { get; set; }
        public string PostOfficeAcceptID { get; set; }
        public string MailerTypeID { get; set; }
    }
    public class ResponseThuHo
    {
        public string MailerID { get; set; }
        public DateTime AcceptDate { get; set; }
        public string SenderID { get; set; }
        public string SenderName { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<double> Weight { get; set; }
        public string ReceiveProvinceID { get; set; }
        public string ServiceTypeID { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Description { get; set; }
        public string PostOfficeAcceptID { get; set; }
        public string PostOfficeRecieverMoneyID { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string DocID { get; set; }
        public string Invoice { get; set; }
        public string PostOfficeID { get; set; }
    }
    public class ResponseTracking
    {
        public string MailerID { get; set; }
        public string StatusName { get; set; }
        public string PostOfficeName { get; set; }
        public string DocumentID { get; set; }
        public string UserGroupID { get; set; }
        public int ID {get;set;}
        public DateTime CreationDate { get; set; }

    }
   public class ResponseTHKT
   {
       public string MailerID { get; set; }
       public Nullable<decimal> Amount {get;set;}
       public string DocID { get; set; }
       public string Invoice { get; set; }
       public string Description { get; set; }
       public DateTime CreateDate { get; set; }
       public string AcceptDate { get; set; }
   }
    public class ResponseSonyMap
    {
        public string Address { get; set; }
        public double? Lang { get; set; }
        public double? Long { get; set; }
    }
    public class ResponesePackingList
    {
        public DateTime AcceptDate {get;set;}
        public string MailerID {get;set;}
        public string PostOfficeID {get;set;}
        public string PostOfficeIDAccept {get;set;}
        public double? Weight{get;set;}
        public string PostOfficeDeliveryID {get;set;}
        public Nullable<DateTime> DeliveryDate { get; set; }
    }
    public class ResponeCheckPackingList
    {
        public string DocumentID {get;set;}
        public string PostOfficeID {get;set;}
        public string PostOfficeIDAccept {get;set;}
        public DateTime DocumentDate {get;set;}
        public string TripNumber {get;set;}
        public double Weight {get;set;}
        public string NumberOfPackage { get; set; }
        public string TransportObjectID {get;set;}
        public string SendDescription { get; set; }
    }
    public class ResponeCheckingPacking
    {
        public string DocumentID { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string PostOfficeIDAccept { get; set; }
        public int NumberOfPackage { get; set; }
        public string TripNumber { get; set; }
        public double? Weight { get; set; }
        public string Description { get; set; }
        public string DocumentOrder { get; set; }
        public string Tranport { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RecieveDate { get; set; }
        public string RecieveDescription { get; set; }
    }

    public class CreateTable
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string IDReceipt { get; set; }
        public string DetailContent { get; set; }
        public string CreateName { get; set; }
        public DateTime CreateDate { get; set; }
        public int IDFault { get; set; }
        public string Status { get; set; }

    }

    public class CustomerGroup
    {
        public string CustomerGroupID {get; set;}
    }

    public class Customer
    {
        public string CustomerID { get; set; }
    }

    public class BaoCaoTongHop
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string CustomerGroupID { get; set; }
        public string CustomerID { get; set; }
        public int TongPhat { get; set; }
        public int TongCG { get; set; }
        public int ChuaPhat { get; set; }
        public DateTime? AcceptDates { get; set; }
        public string MailerID { get; set; }
        public int? Quantity { get; set; }
        public double? Weight { get; set; }
        public string RecieverProvince { get; set; }
        public string Notes { get; set; }
        public string RecieverName { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string CustomerName { get; set; }
        public string DeliveryStatus { get; set; }
    }

    public class DuongTruc_KTNhan
    {
        public string PostOfficeAcceptID { get; set; }
        public int MailerID { get; set; }
        public int Quantity { get; set; }
        public double Weight { get; set; }
        public string RecieverProvinceID { get; set; }
        public string ServiceTypeID { get; set; }
        public string ZoneID { get; set; }
    }

    public class DHLPlan
    {
        public string CG { get; set; }
        public string Contact1 { get; set; }
        public string Contact2 { get; set; }
        public string Contact3 { get; set; }
        public string D_O { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Employee { get; set; }
        public int? ID { get; set; }
        public string KH { get; set; }
        public DateTime? PGI { get; set; }
        public int? Quantity { get; set; }
        public int? SL { get; set; }
        public string SenderAddress { get; set; }
        public string SenderName { get; set; }
        public string ShiptoAddress { get; set; }
        public string ShiptoNM { get; set; }
        public string Subcon { get; set; }
        public int? TL { get; set; }
        public string TP { get; set; }
        public string ToNodeCode { get; set; }
        public string ToZone { get; set; }
        public string TongSL { get; set; }
        public string Unit1 { get; set; }
        public string Unit2 { get; set; }
        public string Unit3 { get; set; }
        public int? Weight { get; set; }
        public string Zone { get; set; }
        public string ZoneDesc { get; set; }
    }

    public class WikiInfo
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public List<WikiInfo> Childes { get; set; }
    }

    public class ResponseMailerByDateDelivery
    {
        public DateTime? AcceptDate { get; set; }
        public string MailerID { get; set; }
        public string SenderID { get; set; }
        public string SenderName { get; set; }
        public string SenderProvinceID { get; set; }
        public string ReceiveProvinceID { get; set; }
        public string RecieverDistrictID { get; set; }
        public string ServiceTypeID { get; set; }
        public string MailerTypeID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<double> RealWeight { get; set; }
        public Nullable<double> Weight { get; set; }
        public Nullable<decimal> Money { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> PriceDefault { get; set; }
        public Nullable<decimal> PriceService { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<decimal> BefVATAmount { get; set; }
        public Nullable<double> VATPercent { get; set; }
        public Nullable<decimal> VATAmount { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> AmountBefDiscount { get; set; }
        public string PostOfficeAcceptID { get; set; }
        public string PaymentMethodID { get; set; }
        public string PostOfficeRecieverMoneyID { get; set; }
        public string MailerDescription { get; set; }
        public string ThirdpartyDocID { get; set; }
        public Nullable<decimal> ThirdpartyCost { get; set; }
        public Nullable<decimal> CommissionAmt { get; set; }
        public Nullable<double> CommissionPercent { get; set; }
        public Nullable<decimal> CostAmt { get; set; }
        public DateTime? SalesClosingDate { get; set; }
        public string RecieverProvinceID { get; set; }
        public Nullable<double> DiscountPercent { get; set; }
        public string PostOfficeID { get; set; }
        public string PostOfficeName { get; set; }
        public string ZoneID { get; set; }
        public string DeliveryPostOfficeID { get; set; }
        public string EmployeeID { get; set; }
    }

    public class BCSanluongNhanhDuoi2
    {
        public string KV { get; set; }

        public int SoLuong { get; set; }

        public double TrongLuong { get; set; }
        public double TrongLuongKhoi { get; set; }
    }

    public class BCSanluongNhanhTren2
    {
        public string KV { get; set; }

        public int SoLuong { get; set; }

        public double TrongLuong { get; set; }
        public double TrongLuongKhoi { get; set; }
    }

    public class BCSanluongThuongDuoi2
    {
        public string KV { get; set; }

        public int SoLuong { get; set; }

        public double TrongLuong { get; set; }
        public double TrongLuongKhoi { get; set; }
    }

    public class BCSanluongThuongTren2
    {
        public string KV { get; set; }

        public int SoLuong { get; set; }

        public double TrongLuong { get; set; }
        public double TrongLuongKhoi { get; set; }
    }

    public class PostOfficeAddress
    {
        public string PostOfficeName { get; set; }

    }

    public class GetAddress
    {
        public string Address { get; set; }

    }

    public class ThongKeSoLieu
    {
        public string MailerID { get; set; }

        public DateTime AcceptDate { get; set; }

        public int Quantity { get; set; }
        public double Weight { get; set; }

        public double RealWeight { get; set; }

        public string RecieverProvinceID { get; set; }
        public string ServiceTypeID { get; set; }

        public decimal Price { get; set; }

        public decimal BefVATAmount { get; set; }
    }

    public class SanLuongNhanDuoi2CPN
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanDuoi2CPT
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanDuoi2EMS
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanDuoi2QT
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanDuoi2CPNKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanDuoi2CPTKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanDuoi2EMSKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanDuoi2QTKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanDuoi2CPNKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanDuoi2CPTKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanDuoi2EMSKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanDuoi2QTKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren2CPN
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren2CPT
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren2EMS
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren2QT
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren2CPNKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren2CPTKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren2EMSKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren2QTKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren2CPNKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren2CPTKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren2EMSKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren2QTKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren8CPN
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren8CPT
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren8EMS
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren8QT
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren8CPNKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren8CPTKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren8EMSKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren8QTKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren8CPNKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren8CPTKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren8EMSKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren8QTKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren50CPN
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren50CPT
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren50EMS
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren50QT
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren50CPNKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren50CPTKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren50EMSKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren50QTKV3
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren50CPNKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren50CPTKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren50EMSKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongNhanTren50QTKV4
    {
        public string PostOfficeAcceptID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }

    public class SanLuongPhatDuoi2CPN
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }
    }
    public class SanLuongPhatTren2CPN
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren8CPN
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren50CPN
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }

    public class SanLuongPhatDuoi2CPT
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }
    }
    public class SanLuongPhatTren2CPT
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren8CPT
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren50CPT
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }

    public class SanLuongPhatDuoi2CPNKV3
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }
    }
    public class SanLuongPhatTren2CPNKV3
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren8CPNKV3
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren50CPNKV3
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }

    public class SanLuongPhatDuoi2CPTKV3
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }
    }
    public class SanLuongPhatTren2CPTKV3
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren8CPTKV3
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren50CPTKV3
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }

    public class SanLuongPhatDuoi2CPNKV4
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }
    }
    public class SanLuongPhatTren2CPNKV4
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren8CPNKV4
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren50CPNKV4
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }

    public class SanLuongPhatDuoi2CPTKV4
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }
    }
    public class SanLuongPhatTren2CPTKV4
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren8CPTKV4
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren50CPTKV4
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }

    public class SanLuongPhatDuoi2CPNKV1
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }
    }
    public class SanLuongPhatTren2CPNKV1
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren8CPNKV1
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren50CPNKV1
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }

    public class SanLuongPhatDuoi2CPTKV1
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }
    }
    public class SanLuongPhatTren2CPTKV1
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren8CPTKV1
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
    public class SanLuongPhatTren50CPTKV1
    {
        public string PostOfficeID { get; set; }
        public int SL { get; set; }
        public double TL { get; set; }
        public decimal DT { get; set; }

    }
}