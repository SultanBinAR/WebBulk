using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBulky.Models.Models
{
	public class OrderHeader
	{
		public int Id { get; set; }
		public string ApplicationUserId { get; set; }
		[ForeignKey(nameof(ApplicationUserId))]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime ShippingDate { get; set; }
		public double OrderTotal { get; set; }

		//Order Status Details
		public string? OrderStatus { get; set; }
		public string? TrackingNumber { get; set; }
		public string? Carrier { get; set; }
		//Payment Rel. including ^ Carrier
		public string? PaymentStatus { get; set; }
		public DateTime PaymentDate { get; set; }
		public DateTime PaymentDueDate { get; set; } //DateOnly & TimeOnly are supported in .Net 8

		public string? SessionId { get; set; }
		//3rd Party Gateways Payment Confirmation ID
		public string? PaymentIntentId { get; set; }

		//User Props.
		[Required]
		public string Name { get; set; }
		[Required]
		public string StreetAddress { get; set; }
		[Required]
		public string City { get; set; }
		[Required]
		public string State { get; set; }
		[Required]
		public string PostalCode { get; set; }
		[Required]
		public string PhoneNumber { get; set; }
	}
}
