﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Xpinn.SportsGo.Entities
{
    using System;
    using System.Collections.Generic;

    public partial class ContactosDTO
    {
        public int Consecutivo { get; set; }
        public int CodigoPersonaOwner { get; set; }
        public int CodigoPersonaContacto { get; set; }

        public virtual PersonasDTO PersonasContacto { get; set; }
        public virtual PersonasDTO PersonasOwner { get; set; }
    }
}