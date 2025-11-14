import { useEffect, useState } from 'react';

interface AutoSelectResult {
  shouldShow: boolean;
  autoSelected: boolean;
  message: string | null;
}

/**
 * Hook para auto-seleção de itens únicos (contas, centros de custo, etc.)
 * 
 * Comportamento:
 * - 0 itens: não mostra campo, retorna null
 * - 1 item: auto-seleciona, mostra mensagem informativa
 * - 2+ itens: mostra campo normalmente
 * 
 * @param itemCount - Quantidade de itens disponíveis
 * @param itemName - Nome do item para mensagem (ex: "conta", "centro de custo")
 * @param selectedItem - Item selecionado (para exibir nome)
 * @returns Objeto com shouldShow, autoSelected e message
 */
export function useAutoSelect(
  itemCount: number,
  itemName: string,
  selectedItem?: { id: number; name: string } | null
): AutoSelectResult {
  const [shouldShow, setShouldShow] = useState(true);
  const [autoSelected, setAutoSelected] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  useEffect(() => {
    if (itemCount === 0) {
      // Sem itens: não mostrar campo
      setShouldShow(false);
      setAutoSelected(false);
      setMessage(`Nenhum${itemName === 'conta' ? 'a' : ''} ${itemName} cadastrad${itemName === 'conta' ? 'a' : 'o'}`);
    } else if (itemCount === 1) {
      // 1 item: auto-selecionar
      setShouldShow(false);
      setAutoSelected(true);
      if (selectedItem) {
        setMessage(`${itemName.charAt(0).toUpperCase() + itemName.slice(1)} selecionad${itemName === 'conta' ? 'a' : 'o'} automaticamente: ${selectedItem.name}`);
      } else {
        setMessage(`${itemName.charAt(0).toUpperCase() + itemName.slice(1)} únic${itemName === 'conta' ? 'a' : 'o'} selecionad${itemName === 'conta' ? 'a' : 'o'} automaticamente`);
      }
    } else {
      // 2+ itens: mostrar normalmente
      setShouldShow(true);
      setAutoSelected(false);
      setMessage(null);
    }
  }, [itemCount, itemName, selectedItem]);

  return { shouldShow, autoSelected, message };
}
