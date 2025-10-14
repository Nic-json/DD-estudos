using Microsoft.AspNetCore.Identity;

namespace rebuild.Models
    {
        public class PortugueseIdentityErrorDescriber : IdentityErrorDescriber
        {
            public override IdentityError PasswordTooShort(int length) =>
                new IdentityError
                {
                    Code = nameof(PasswordTooShort),
                    Description = $"A senha deve ter pelo menos {length} caractere(s)."
                };

            public override IdentityError PasswordRequiresNonAlphanumeric() =>
                new IdentityError
                {
                    Code = nameof(PasswordRequiresNonAlphanumeric),
                    Description = "A senha deve conter pelo menos um caractere não alfanumérico."
                };

            public override IdentityError PasswordRequiresDigit() =>
                new IdentityError
                {
                    Code = nameof(PasswordRequiresDigit),
                    Description = "A senha deve conter pelo menos um dígito ('0'-'9')."
                };

            public override IdentityError PasswordRequiresLower() =>
                new IdentityError
                {
                    Code = nameof(PasswordRequiresLower),
                    Description = "A senha deve conter pelo menos uma letra minúscula ('a'-'z')."
                };

            public override IdentityError PasswordRequiresUpper() =>
                new IdentityError
                {
                    Code = nameof(PasswordRequiresUpper),
                    Description = "A senha deve conter pelo menos uma letra maiúscula ('A'-'Z')."
                };

            // Adicione outros overrides se desejar (DuplicateEmail, InvalidEmail, etc.)
        }
    }
