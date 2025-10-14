using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Modelo.Discente
{
    public class Academico
    {
        public long? AcademicoID { get; set; }

        [DisplayName("RA")]
        [Required(ErrorMessage = "Informe o RA.")]
        // Um único validador que garante exatamente 10 dígitos numéricos
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "O RA deve conter exatamente 10 dígitos numéricos.")]
        public string RegistroAcademico { get; set; }

        [DisplayName("Nome")]
        [Required(ErrorMessage = "Informe o nome.")]
        [StringLength(100, ErrorMessage = "O Nome pode ter no máximo {1} caracteres.")]
        public string Nome { get; set; }

        [DisplayName("Nascimento")]
        [Required(ErrorMessage = "Informe a data de nascimento.")]
        [DataType(DataType.Date, ErrorMessage = "Data inválida.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Nascimento { get; set; }

        public string? FotoMimeType { get; set; }
        public byte[]? Foto { get; set; }

        [NotMapped]
        [DisplayName("Foto (opcional)")]
        public IFormFile? FormFile { get; set; }
    }
}