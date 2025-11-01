using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TechDesk.Models;

namespace TechDesk.Data;

public partial class TechDeskDbContext : DbContext
{
    public TechDeskDbContext()
    {
    }

    public TechDeskDbContext(DbContextOptions<TechDeskDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnexosMensagem> AnexosMensagems { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Chamado> Chamados { get; set; }

    public virtual DbSet<Faq> Faqs { get; set; }

    public virtual DbSet<FeedbackAtendimento> FeedbackAtendimentos { get; set; }

    public virtual DbSet<HistoricoChamado> HistoricoChamados { get; set; }

    public virtual DbSet<PreferenciasNotificacao> PreferenciasNotificacaos { get; set; }

    public virtual DbSet<SolucoesSugerida> SolucoesSugeridas { get; set; }

    public virtual DbSet<Tecnico> Tecnicos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VwResumoPorStatus> VwResumoPorStatuses { get; set; }

    public virtual DbSet<VwTma> VwTmas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => base.OnConfiguring(optionsBuilder);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnexosMensagem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_AnexosMensagem");

            entity.ToTable("Anexos_Mensagem", "TechDesk");

            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.DataUpload)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Descricao).HasMaxLength(200);
            entity.Property(e => e.NomeArquivo).HasMaxLength(260);
            entity.Property(e => e.Url).HasMaxLength(500);

            entity.HasOne(d => d.IdMensagemNavigation).WithMany(p => p.AnexosMensagems)
                .HasForeignKey(d => d.IdMensagem)
                .HasConstraintName("FK_AnexoMsg_Mensagem");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria);

            entity.ToTable("Categorias", "TechDesk");

            entity.Property(e => e.IdCategoria).HasColumnName("Id_Categoria");
            entity.Property(e => e.Ativa).HasDefaultValue(true);
            entity.Property(e => e.Descricao).HasMaxLength(500);
            entity.Property(e => e.Nome).HasMaxLength(120);
        });

        modelBuilder.Entity<Chamado>(static entity =>
        {
            entity.HasKey(e => e.IdChamado);

            entity.ToTable("Chamados", "TechDesk");

            entity.HasIndex(e => e.IdCategoria, "IX_Chamados_Categoria");

            entity.HasIndex(e => new { e.DataInicio, e.DataFinal }, "IX_Chamados_Datas");

            entity.HasIndex(e => e.Prioridade, "IX_Chamados_Prioridade");

            entity.HasIndex(e => e.Status, "IX_Chamados_Status");

            entity.HasIndex(e => e.IdTecnico, "IX_Chamados_Tecnico");

            entity.HasIndex(e => e.IdUsuario, "IX_Chamados_Usuario");

            entity.Property(e => e.IdChamado).HasColumnName("Id_Chamado");
            entity.Property(e => e.DataFinal).HasPrecision(0);
            entity.Property(e => e.DataInicio)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Nivel).HasMaxLength(10);
            entity.Property(e => e.Prioridade).HasMaxLength(10);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Titulo).HasMaxLength(200);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Chamados)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chamados_Categoria");

            entity.HasOne(d => d.IdTecnicoNavigation).WithMany(p => p.Chamados)
                .HasForeignKey(d => d.IdTecnico)
                .HasConstraintName("FK_Chamados_Tecnico");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(static p => p.Chamados)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chamados_Usuario");
        });

        modelBuilder.Entity<Faq>(entity =>
        {
            entity.ToTable("Faq", "TechDesk");

            entity.Property(e => e.Pergunta).HasMaxLength(400);

            entity.HasOne(d => d.Categoria).WithMany(p => p.Faqs)
                .HasForeignKey(d => d.CategoriaId)
                .HasConstraintName("FK_Faq_Categoria");
        });

        modelBuilder.Entity<FeedbackAtendimento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Feedback");

            entity.ToTable("Feedback_Atendimento", "TechDesk");

            entity.HasIndex(e => new { e.IdChamado, e.UsuarioId }, "UX_Feedback_Unico").IsUnique();

            entity.Property(e => e.Comentario).HasMaxLength(1000);
            entity.Property(e => e.Data)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.IdChamadoNavigation).WithMany(p => p.FeedbackAtendimentos)
                .HasForeignKey(d => d.IdChamado)
                .HasConstraintName("FK_Feedback_Chamado");

            entity.HasOne(d => d.Usuario).WithMany(p => p.FeedbackAtendimentos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Feedback_Usuario");
        });

        modelBuilder.Entity<HistoricoChamado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Historico");

            entity.ToTable("Historico_Chamado", "TechDesk");

            entity.Property(e => e.AutorTipo).HasMaxLength(10);
            entity.Property(e => e.Data)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.StatusAntes).HasMaxLength(20);
            entity.Property(e => e.StatusDepois).HasMaxLength(20);
            entity.Property(e => e.Visibilidade).HasMaxLength(10);

            entity.HasOne(d => d.AutorTecnico).WithMany(p => p.HistoricoChamados)
                .HasForeignKey(d => d.AutorTecnicoId)
                .HasConstraintName("FK_Historico_Tecnico");

            entity.HasOne(d => d.AutorUsuario).WithMany(p => p.HistoricoChamados)
                .HasForeignKey(d => d.AutorUsuarioId)
                .HasConstraintName("FK_Historico_Usuario");

            entity.HasOne(d => d.IdChamadoNavigation).WithMany(p => p.HistoricoChamados)
                .HasForeignKey(d => d.IdChamado)
                .HasConstraintName("FK_Historico_Chamado");
        });

        modelBuilder.Entity<PreferenciasNotificacao>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Prefs");

            entity.ToTable("Preferencias_Notificacao", "TechDesk");

            entity.HasIndex(e => e.UsuarioId, "UQ_Prefs_Usuario").IsUnique();

            entity.Property(e => e.AtualizadoEm)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasDefaultValue(true);
            entity.Property(e => e.StatusUpdates).HasDefaultValue(true);

            entity.HasOne(d => d.Usuario).WithOne(p => p.PreferenciasNotificacao)
                .HasForeignKey<PreferenciasNotificacao>(d => d.UsuarioId)
                .HasConstraintName("FK_Prefs_Usuario");
        });

        modelBuilder.Entity<SolucoesSugerida>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Sugestoes");

            entity.ToTable("Solucoes_Sugeridas", "TechDesk");

            entity.Property(e => e.Confianca).HasColumnType("decimal(5, 4)");
            entity.Property(e => e.CriadaEm)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.PassoApasso).HasColumnName("PassoAPasso");
            entity.Property(e => e.Titulo).HasMaxLength(200);

            entity.HasOne(d => d.IdChamadoNavigation).WithMany(p => p.SolucoesSugerida)
                .HasForeignKey(d => d.IdChamado)
                .HasConstraintName("FK_Sugestoes_Chamado");
        });

        modelBuilder.Entity<Tecnico>(entity =>
        {
            entity.ToTable("Tecnicos", "TechDesk");

            entity.HasIndex(e => e.Email, "UQ_Tecnicos_Email").IsUnique();

            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.AtualizadoEm).HasPrecision(0);
            entity.Property(e => e.CodigoEmpresa).HasMaxLength(40);
            entity.Property(e => e.CriadoEm)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(180);
            entity.Property(e => e.Especialidade).HasMaxLength(120);
            entity.Property(e => e.Nivel).HasMaxLength(10);
            entity.Property(e => e.Nome).HasMaxLength(120);
            entity.Property(e => e.Perfil)
                .HasMaxLength(40)
                .HasDefaultValue("Técnico");
            entity.Property(e => e.SenhaHash).HasMaxLength(255);

            entity.HasMany(d => d.Categoria).WithMany(p => p.Tecnicos)
                .UsingEntity<Dictionary<string, object>>(
                    "TecnicoCategoria",
                    r => r.HasOne<Categoria>().WithMany()
                        .HasForeignKey("CategoriaId")
                        .HasConstraintName("FK_TecCat_Categoria"),
                    l => l.HasOne<Tecnico>().WithMany()
                        .HasForeignKey("TecnicoId")
                        .HasConstraintName("FK_TecCat_Tecnico"),
                    j =>
                    {
                        j.HasKey("TecnicoId", "CategoriaId");
                        j.ToTable("Tecnico_Categorias", "TechDesk");
                    });
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios", "TechDesk");

            entity.HasIndex(e => e.Email, "UQ_Usuarios_Email").IsUnique();

            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.AtualizadoEm).HasPrecision(0);
            entity.Property(e => e.CriadoEm)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(180);
            entity.Property(e => e.Nome).HasMaxLength(120);
            entity.Property(e => e.Perfil).HasMaxLength(40);
            entity.Property(e => e.SenhaHash).HasMaxLength(255);
        });

        modelBuilder.Entity<VwResumoPorStatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ResumoPorStatus", "TechDesk");

            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<VwTma>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_TMA", "TechDesk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
