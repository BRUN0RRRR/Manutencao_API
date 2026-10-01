using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace manutencao_api.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Equipamento> Equipamentos { get; set; }

    public virtual DbSet<Grupo> Grupos { get; set; }

    public virtual DbSet<HistoricoO> HistoricoOs { get; set; }

    public virtual DbSet<OrdemServico> OrdemServicos { get; set; }

    public virtual DbSet<OrdemServicoAnexo> OrdemServicoAnexos { get; set; }

    public virtual DbSet<Perfi> Perfis { get; set; }

    public virtual DbSet<Setore> Setores { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=manutencao;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Equipame__3214EC07C423EDA3");

            entity.HasIndex(e => e.Patrimonio, "UQ__Equipame__844CF2005FFF0D2C").IsUnique();

            entity.Property(e => e.Nome)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Patrimonio)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Ativo");

            entity.HasOne(d => d.Setor).WithMany(p => p.Equipamentos)
                .HasForeignKey(d => d.SetorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Equipamen__Setor__6E01572D");
        });

        modelBuilder.Entity<Grupo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Grupos__3214EC0737350035");

            entity.HasIndex(e => e.Nome, "UQ__Grupos__7D8FE3B235C1E724").IsUnique();

            entity.Property(e => e.Descricao)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HistoricoO>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historic__3214EC072638D062");

            entity.ToTable("HistoricoOS");

            entity.Property(e => e.Comentario).HasColumnType("text");
            entity.Property(e => e.DataRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.OrdemServico).WithMany(p => p.HistoricoOs)
                .HasForeignKey(d => d.OrdemServicoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historico__Ordem__7F2BE32F");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistoricoOs)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historico__Usuar__00200768");
        });

        modelBuilder.Entity<OrdemServico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrdemSer__3214EC07D81D55CD");

            entity.ToTable("OrdemServico");

            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Geral");
            entity.Property(e => e.DataAbertura)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DataFechamento).HasColumnType("datetime");
            entity.Property(e => e.Descricao).HasColumnType("text");
            entity.Property(e => e.PrazoResolucao).HasColumnType("datetime");
            entity.Property(e => e.Prioridade)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Normal");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Aberta");
            entity.Property(e => e.Titulo)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Equipamento).WithMany(p => p.OrdemServicos)
                .HasForeignKey(d => d.EquipamentoId)
                .HasConstraintName("FK__OrdemServ__Equip__787EE5A0");

            entity.HasOne(d => d.GrupoResponsavel).WithMany(p => p.OrdemServicos)
                .HasForeignKey(d => d.GrupoResponsavelId)
                .HasConstraintName("FK__OrdemServ__Grupo__7B5B524B");

            entity.HasOne(d => d.Requerente).WithMany(p => p.OrdemServicoRequerentes)
                .HasForeignKey(d => d.RequerenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrdemServ__Reque__797309D9");

            entity.HasOne(d => d.Tecnico).WithMany(p => p.OrdemServicoTecnicos)
                .HasForeignKey(d => d.TecnicoId)
                .HasConstraintName("FK__OrdemServ__Tecni__7A672E12");
        });

        modelBuilder.Entity<OrdemServicoAnexo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrdemSer__3214EC0735DA4B2D");

            entity.Property(e => e.CaminhoArquivo)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.DataUpload)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NomeOriginalArquivo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TipoArquivo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.OrdemServico).WithMany(p => p.OrdemServicoAnexos)
                .HasForeignKey(d => d.OrdemServicoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrdemServ__Ordem__03F0984C");

            entity.HasOne(d => d.Uploader).WithMany(p => p.OrdemServicoAnexos)
                .HasForeignKey(d => d.UploaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrdemServ__Uploa__04E4BC85");
        });

        modelBuilder.Entity<Perfi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Perfis__3214EC07A5A013C7");

            entity.HasIndex(e => e.Nome, "UQ__Perfis__7D8FE3B2CEDB7EF2").IsUnique();

            entity.Property(e => e.NivelAcesso).HasDefaultValue(1);
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Setore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Setores__3214EC07A0F222E6");

            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC07D682ACAF");

            entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D10534422DCAF3").IsUnique();

            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.Cargo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DataCriacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Ramal)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SenhaHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Telefone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UltimoLogin).HasColumnType("datetime");

            entity.HasOne(d => d.PerfilBase).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.PerfilBaseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuarios__Perfil__68487DD7");

            entity.HasOne(d => d.Setor).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.SetorId)
                .HasConstraintName("FK__Usuarios__SetorI__693CA210");

            entity.HasMany(d => d.Grupos).WithMany(p => p.Usuarios)
                .UsingEntity<Dictionary<string, object>>(
                    "UsuarioGrupo",
                    r => r.HasOne<Grupo>().WithMany()
                        .HasForeignKey("GrupoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UsuarioGr__Grupo__71D1E811"),
                    l => l.HasOne<Usuario>().WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UsuarioGr__Usuar__70DDC3D8"),
                    j =>
                    {
                        j.HasKey("UsuarioId", "GrupoId").HasName("PK__UsuarioG__3E6B58BC26E8EA29");
                        j.ToTable("UsuarioGrupos");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
