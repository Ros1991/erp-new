# Páginas de Autenticação

Este diretório contém todas as páginas relacionadas à autenticação e acesso ao sistema.

## Páginas

- **Landing.tsx** - Página institucional inicial
- **Login.tsx** - Tela de login (aceita email, telefone ou CPF)
- **Register.tsx** - Tela de cadastro de novos usuários
- **ForgotPassword.tsx** - Recuperação de senha

## Estrutura de Dados

### Login
- **Credential**: Email, Telefone ou CPF (string)
- **Password**: Senha do usuário (mínimo 6 caracteres)

### Register
- **Email**: Opcional (máx 255 caracteres)
- **Phone**: Opcional (máx 20 caracteres)  
- **Cpf**: Opcional (11 dígitos)
- **Password**: Obrigatório (mínimo 6 caracteres)

**Validação**: Pelo menos um dos três campos (Email, Phone ou CPF) deve ser preenchido.
