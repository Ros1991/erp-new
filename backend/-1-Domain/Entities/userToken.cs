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
		public string Token { get; set; }

		[Column("user_token_refresh_token")]
		public string RefreshToken { get; set; }

		[Column("user_token_expires_at")]
		public string ExpiresAt { get; set; }

		[Column("user_token_refresh_expires_at")]
		public string RefreshExpiresAt { get; set; }

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
			string Param_RefreshToken, 
			string Param_ExpiresAt, 
			string Param_RefreshExpiresAt, 
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
