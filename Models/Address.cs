﻿namespace ActiveX_Api.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Street { get; set; }
        public string? Number { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? UserId { get; set; }
        public ApiUser User { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
