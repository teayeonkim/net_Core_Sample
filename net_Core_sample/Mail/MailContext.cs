﻿using Microsoft.EntityFrameworkCore;
using System;

namespace Mail
{
    public class MailContext : DbContext
    {

        public MailContext(DbContextOptions<MailContext> options) : base(options)
        {

        }

        public virtual DbSet<tbl_Mail> tbl_Mail { get; set; }
    }
}
