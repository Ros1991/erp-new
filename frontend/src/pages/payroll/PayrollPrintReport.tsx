import { forwardRef } from 'react';
import type { PayrollDetailed, PayrollEmployeeDetailed } from '../../services/payrollService';

interface PayrollPrintReportProps {
  payroll: PayrollDetailed;
  companyName: string;
  inssAmount: number;
  fgtsAmount: number;
}

export const PayrollPrintReport = forwardRef<HTMLDivElement, PayrollPrintReportProps>(
  ({ payroll, companyName, inssAmount, fgtsAmount }, ref) => {
    const formatCurrency = (valueInCents: number) => {
      return new Intl.NumberFormat('pt-BR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      }).format(valueInCents / 100);
    };

    const formatPeriod = (startDate: string) => {
      const date = new Date(startDate);
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const year = date.getFullYear();
      return `${month}/${year}`;
    };

    // Calcular totais de acréscimos (proventos exceto salário base)
    const totalAcrescimos = payroll.employees.reduce((total, emp) => {
      return total + emp.items
        .filter(item => item.type === 'Provento' && item.category !== 'Salario')
        .reduce((sum, item) => sum + item.amount, 0);
    }, 0);

    return (
      <div ref={ref} className="print-report bg-white p-6 text-black text-sm">
        <style>{`
          @media print {
            body * {
              visibility: hidden;
            }
            .print-report, .print-report * {
              visibility: visible;
            }
            .print-report {
              position: absolute;
              left: 0;
              top: 0;
              width: 100%;
              font-size: 11px;
              padding: 10px;
            }
            .no-print {
              display: none !important;
            }
            .page-break {
              page-break-before: always;
            }
          }
        `}</style>

        {/* Header */}
        <div className="text-center mb-4 border-b pb-4">
          <h1 className="text-2xl font-bold">{companyName}</h1>
          <p className="text-gray-600">Extrato da folha de pagamento</p>
        </div>

        {/* Resumo */}
        <div className="flex justify-between mb-6 border-b pb-4">
          <div className="space-y-1">
            <div className="flex gap-8">
              <span className="text-gray-600">Valor Total:</span>
              <span className="font-semibold">{formatCurrency(payroll.totalGrossPay)}</span>
            </div>
            <div className="flex gap-8">
              <span className="text-gray-600">Acréscimos/Descontos:</span>
              <span className="font-semibold">{formatCurrency(totalAcrescimos)}</span>
            </div>
            <div className="flex gap-8">
              <span className="text-gray-600">Valor Líquido:</span>
              <span className="font-bold">{formatCurrency(payroll.totalNetPay)}</span>
            </div>
          </div>
          <div className="text-right">
            <div className="text-lg">
              <span className="text-gray-600">Referência: </span>
              <span className="font-bold text-xl">{formatPeriod(payroll.periodStartDate)}</span>
            </div>
            <div className="mt-2 space-y-1 text-right">
              <div>
                <span className="text-gray-600">Valor FGTS a recolher:</span>
                <span className="ml-2 font-semibold">{formatCurrency(fgtsAmount)}</span>
              </div>
              <div>
                <span className="text-gray-600">Valor INSS a recolher:</span>
                <span className="ml-2 font-semibold">{formatCurrency(inssAmount)}</span>
              </div>
            </div>
          </div>
        </div>

        {/* Lista de Funcionários */}
        <div className="space-y-4">
          {payroll.employees.map((employee) => (
            <EmployeeRow key={employee.payrollEmployeeId} employee={employee} formatCurrency={formatCurrency} />
          ))}
        </div>
      </div>
    );
  }
);

PayrollPrintReport.displayName = 'PayrollPrintReport';

// Componente para cada funcionário
function EmployeeRow({ 
  employee, 
  formatCurrency 
}: { 
  employee: PayrollEmployeeDetailed; 
  formatCurrency: (value: number) => string;
}) {
  const proventos = employee.items.filter(item => item.type === 'Provento');
  const descontos = employee.items.filter(item => item.type === 'Desconto');
  
  // Combinar itens para mostrar lado a lado
  const maxItems = Math.max(proventos.length, descontos.length);

  return (
    <div className="border-b pb-3 mb-3">
      <div className="flex justify-between items-start">
        {/* Nome e itens */}
        <div className="flex-1">
          <h3 className="font-bold text-base">{employee.employeeName}</h3>
          <div className="mt-1 text-xs space-y-0.5">
            {Array.from({ length: maxItems }).map((_, index) => {
              const provento = proventos[index];
              const desconto = descontos[index];
              return (
                <div key={index} className="flex gap-4">
                  {provento ? (
                    <span className="text-gray-700 w-64">
                      {provento.description} - {provento.category}
                    </span>
                  ) : (
                    <span className="w-64"></span>
                  )}
                  {provento ? (
                    <span className="w-20 text-right">
                      <span className="text-green-700">C</span> {formatCurrency(provento.amount)}
                    </span>
                  ) : (
                    <span className="w-20"></span>
                  )}
                  {desconto ? (
                    <span className="text-gray-700 w-48">
                      {desconto.description}
                    </span>
                  ) : (
                    <span className="w-48"></span>
                  )}
                  {desconto ? (
                    <span className="w-20 text-right">
                      <span className="text-red-700">D</span> {formatCurrency(desconto.amount)}
                    </span>
                  ) : (
                    <span className="w-20"></span>
                  )}
                </div>
              );
            })}
          </div>
        </div>

        {/* Valor Líquido e dados */}
        <div className="text-right ml-4">
          <div className="font-bold">
            <span className="text-gray-600">Valor Líquido: </span>
            <span className="text-lg">{formatCurrency(employee.totalNetPay)}</span>
          </div>
          <div className="text-xs text-gray-600 mt-1">
            <div>CPF: {employee.employeeDocument || '-'}</div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default PayrollPrintReport;
