using System.ComponentModel.DataAnnotations;

namespace IbgeStats.DTOs
{
    public class CreatePesquisaDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(500, ErrorMessage = "Nome deve ter no máximo 500 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Contexto deve ter no máximo 50 caracteres")]
        public string Contexto { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Categoria deve ter no máximo 100 caracteres")]
        public string Categoria { get; set; } = "Geral";
    }
}