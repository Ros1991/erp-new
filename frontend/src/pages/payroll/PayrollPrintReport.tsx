import { forwardRef } from 'react';
import type { PayrollDetailed, PayrollEmployeeDetailed } from '../../services/payrollService';

interface PayrollPrintReportProps {
  payroll: PayrollDetailed;
  companyName: string;
}

export const PayrollPrintReport = forwardRef<HTMLDivElement, PayrollPrintReportProps>(
  ({ payroll, companyName }, ref) => {
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

    // Calcular total de salários (proventos com categoria Salario)
    const totalSalarios = payroll.employees.reduce((total, emp) => {
      return total + emp.items
        .filter(item => item.type === 'Provento' && item.category === 'Salario')
        .reduce((sum, item) => sum + item.amount, 0);
    }, 0);

    // Calcular totais de acréscimos (proventos exceto salário base)
    const totalAcrescimos = payroll.employees.reduce((total, emp) => {
      return total + emp.items
        .filter(item => item.type === 'Provento' && item.category !== 'Salario')
        .reduce((sum, item) => sum + item.amount, 0);
    }, 0);

    return (
      <div ref={ref} id="print-report" className="print-report-container">
        <style>{`
          @media print {
            body > #root > div:first-child {
              display: none !important;
            }
            .print-report-container {
              display: block !important;
            }
            .employee-row {
              page-break-inside: avoid;
            }
            @page {
              margin: 8mm;
            }
          }
          @media screen {
            .print-report-container {
              display: none;
            }
          }
        `}</style>

        <div className="bg-white p-4 text-black" style={{ fontSize: '10px', lineHeight: '1.3' }}>
          {/* Header */}
          <div className="text-center border-b border-black pb-2 mb-3">
            <h1 className="text-xl font-bold">{companyName}</h1>
            <p className="text-gray-600 text-xs">Extrato da folha de pagamento</p>
          </div>

          {/* Resumo em Grid */}
          <div className="grid grid-cols-3 gap-2 mb-3 border-b border-black pb-3" style={{ fontSize: '9px' }}>
            <div>
              <div><span className="text-gray-600">Referência:</span> <strong>{formatPeriod(payroll.periodStartDate)}</strong></div>
              <div><span className="text-gray-600">Empregados:</span> <strong>{payroll.employeeCount}</strong></div>
            </div>
            <div>
              <div><span className="text-gray-600">Total Salários:</span> <strong className="text-green-700">{formatCurrency(totalSalarios)}</strong></div>
              <div><span className="text-gray-600">Total Benefícios:</span> <strong className="text-green-700">{formatCurrency(totalAcrescimos)}</strong></div>
              <div><span className="text-gray-600">Total Deduções:</span> <strong className="text-red-700">{formatCurrency(payroll.totalDeductions)}</strong></div>
            </div>
            <div>
              <div><span className="text-gray-600">Total Líquido:</span> <strong className="text-green-700">{formatCurrency(payroll.totalNetPay)}</strong></div>
              <div><span className="text-gray-600">INSS a Recolher:</span> <strong>{formatCurrency(payroll.totalInss)}</strong></div>
              <div><span className="text-gray-600">FGTS a Recolher:</span> <strong>{formatCurrency(payroll.totalFgts)}</strong></div>
            </div>
          </div>

          {/* Observações */}
          {payroll.notes && (
            <div className="mb-3 border-b border-gray-300 pb-2" style={{ fontSize: '9px' }}>
              <span className="text-gray-600">Obs:</span> {payroll.notes}
            </div>
          )}

          {/* Lista de Funcionários */}
          <div>
            {payroll.employees.map((employee) => (
              <EmployeeRow key={employee.payrollEmployeeId} employee={employee} formatCurrency={formatCurrency} />
            ))}
          </div>
        </div>
      </div>
    );
  }
);

PayrollPrintReport.displayName = 'PayrollPrintReport';

// Componente para cada funcionário - layout com itens abaixo do nome
function EmployeeRow({ 
  employee, 
  formatCurrency 
}: { 
  employee: PayrollEmployeeDetailed; 
  formatCurrency: (value: number) => string;
}) {
  const proventos = employee.items.filter(item => item.type === 'Provento');
  const descontos = employee.items.filter(item => item.type === 'Desconto');

  return (
    <div className="employee-row border-b border-gray-300 py-2 mb-1" style={{ fontSize: '9px' }}>
      {/* Cabeçalho: Nome + Total */}
      <div className="flex justify-between items-center mb-1">
        <div className="font-bold" title={employee.employeeName}>
          {employee.employeeName}
        </div>
        <div className="font-bold text-right">
          Líquido: {formatCurrency(employee.totalNetPay)}
        </div>
      </div>

      {/* Proventos e Descontos lado a lado */}
      <div className="flex gap-4 pl-2">
        {/* Proventos */}
        <div className="flex-1">
          {proventos.map((item, idx) => (
            <div key={idx} className="flex justify-between">
              <span className="text-gray-600">{item.description}</span>
              <span className="text-green-700">{formatCurrency(item.amount)}</span>
            </div>
          ))}
        </div>

        {/* Descontos */}
        <div className="flex-1">
          {descontos.map((item, idx) => (
            <div key={idx} className="flex justify-between">
              <span className="text-gray-600">{item.description}</span>
              <span className="text-red-700">-{formatCurrency(item.amount)}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

export default PayrollPrintReport;
