using Pagamento.Domain.Entities;
using Pagamento.Domain.Exceptions;
using TechTalk.SpecFlow;

namespace Pagamento.Tests.BDD.Step
{
    [Binding]
    public class PagamentoSteps
    {
        private Domain.Entities.Pagamento _pagamento;
        private Exception _exception;

        [Given(@"que o valor do pagamento é (.*)")]
        public void DadoQueOValorDoPagamentoE(decimal valorTotal)
        {
            try
            {
                _pagamento = new Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Pix, valorTotal, null);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [Given(@"o método de pagamento é (.*)")]
        public void DadoQueOMetodoDePagamentoE(string metodo)
        {
            try
            {
                if (!Enum.TryParse<MetodoDePagamento>(metodo, out var metodoDePagamento))
                {
                    throw new ArgumentException($"Método de pagamento inválido: {metodo}");
                }

                _pagamento = new Domain.Entities.Pagamento(Guid.NewGuid(), Guid.NewGuid(), metodoDePagamento, 100.00m, null);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }


        [When(@"eu tentar criar um pagamento")]
        public void QuandoEuTentarCriarUmPagamento()
        {
            try
            {
                _pagamento ??= new Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Pix, 100.00m, null);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [When(@"eu criar um pagamento")]
        public void QuandoEuCriarUmPagamento()
        {
            try
            {
                _pagamento = new Domain.Entities.Pagamento(Guid.NewGuid(), MetodoDePagamento.Pix, 150.00m, null);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [Then(@"uma exceção do tipo DomainExceptionValidation deve ser lançada")]
        public void EntaoUmaExcecaoDeValidacaoDeveSerLancada()
        {
            Assert.NotNull(_exception);
            Assert.IsType<DomainExceptionValidation>(_exception);
        }

        [Then(@"o pagamento deve ser criado com sucesso")]
        public void EntaoOPagamentoDeveSerCriadoComSucesso()
        {
            Assert.NotNull(_pagamento);
           
        }
    }
}
