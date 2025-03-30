using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Domain.Models;

namespace Infrastructure.Contexts.Config
{
    public class SalaryFormulaConfigConfig : IEntityTypeConfiguration<SalaryFormulaConfig>
    {
        public void Configure(EntityTypeBuilder<SalaryFormulaConfig> entity)
        {
            entity.ToTable("SalaryFormulaConfig");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Type)
                .IsRequired()
                .HasColumnType("varchar(50)");

            entity.Property(x => x.Expression)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            entity.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(x => x.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            entity.Property(x => x.CreatedBy)
                .HasColumnType("nvarchar(500)");

            entity.HasIndex(x => x.Type);
        }
    }
}
