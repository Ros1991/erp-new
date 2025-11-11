//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_user_token", Schema = "erp")]
	public class UserToken
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("user_token_id")]
		public long UserTokenId { get; set; }

		[Column("user_id")]
		public long UserId { get; set; }

		[Column("user_token_token")]
		[MaxLength(2048)]
		public string Token { get; set; }

		[Column("user_token_refresh_token")]
		[MaxLength(2048)]
		public string? RefreshToken { get; set; }

		[Column("user_token_expires_at")]
		public DateTime ExpiresAt { get; set; }

		[Column("user_token_refresh_expires_at")]
		public DateTime? RefreshExpiresAt { get; set; }

		[Column("user_token_is_revoked")]
		public bool IsRevoked { get; set; }


		//Criando Relação com a tabelas
        public virtual User User { get; set; } = null!;
		// Construtor padrão para EF
		public UserToken() { }

		// Construtor com parâmetros
		public UserToken(
			long Param_UserId, 
			string Param_Token, 
			string? Param_RefreshToken, 
			DateTime Param_ExpiresAt, 
			DateTime? Param_RefreshExpiresAt, 
			bool Param_IsRevoked
		)
		{
			UserId = Param_UserId;
			Token = Param_Token;
			RefreshToken = Param_RefreshToken;
			ExpiresAt = Param_ExpiresAt;
			RefreshExpiresAt = Param_RefreshExpiresAt;
			IsRevoked = Param_IsRevoked;
		}
	}
}
