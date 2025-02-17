# language: pt-BR
Funcionalidade: Gerenciamento de Pagamentos
  Para garantir a consistência dos dados
  Como um desenvolvedor
  Quero validar as regras de negócio ao criar um pagamento

Cenário: Deve criar pagamento com sucesso
    Dado que o valor do pagamento é 150.00
    E o método de pagamento é "Pix"
    Quando eu criar um pagamento
    Então o pagamento deve ser criado com sucesso

