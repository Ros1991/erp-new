import { useState, useRef, type ReactNode } from 'react';
import { Trash2 } from 'lucide-react';

interface SwipeToDeleteProps {
  children: ReactNode;
  onDelete: () => void;
  onTap?: () => void;
  disabled?: boolean;
}

export function SwipeToDelete({ children, onDelete, onTap, disabled = false }: SwipeToDeleteProps) {
  const [showDeleteOverlay, setShowDeleteOverlay] = useState(false);
  const startTime = useRef(0);
  const longPressTimer = useRef<number | null>(null);
  const hasMoved = useRef(false);

  const handleTouchStart = () => {
    if (disabled) return;
    startTime.current = Date.now();
    hasMoved.current = false;
    
    // Long press detector
    longPressTimer.current = window.setTimeout(() => {
      setShowDeleteOverlay(true);
    }, 500) as unknown as number;
  };

  const handleTouchMove = () => {
    hasMoved.current = true;
    // Cancela long press se mover
    if (longPressTimer.current !== null) {
      window.clearTimeout(longPressTimer.current);
      longPressTimer.current = null;
    }
  };

  const handleTouchEnd = () => {
    if (disabled) return;
    
    // Cancela long press timer
    if (longPressTimer.current !== null) {
      window.clearTimeout(longPressTimer.current);
      longPressTimer.current = null;
    }
    
    const tapDuration = Date.now() - startTime.current;
    
    // Se foi um tap rápido sem movimento
    if (!hasMoved.current && tapDuration < 300 && !showDeleteOverlay) {
      if (onTap) onTap();
    }
  };

  const handleDeleteClick = (e: React.MouseEvent) => {
    e.stopPropagation();
    onDelete();
    setShowDeleteOverlay(false);
  };

  return (
    <div className="relative">
      <div
        className="relative rounded-lg"
        onTouchStart={handleTouchStart}
        onTouchMove={handleTouchMove}
        onTouchEnd={handleTouchEnd}
      >
        {children}
      </div>

      {/* Delete Button Overlay - long press */}
      {showDeleteOverlay && (
        <div 
          className="absolute inset-0 bg-black bg-opacity-60 flex items-center justify-center rounded-lg animate-in fade-in duration-200 z-10"
          onClick={(e) => {
            if (e.target === e.currentTarget) {
              setShowDeleteOverlay(false);
            }
          }}
        >
          <button
            onClick={handleDeleteClick}
            className="bg-red-600 hover:bg-red-700 active:bg-red-800 text-white px-6 py-3 rounded-lg shadow-lg flex items-center gap-2 transition-all transform active:scale-95"
          >
            <Trash2 className="h-5 w-5" />
            <span className="font-medium">Excluir</span>
          </button>
        </div>
      )}
    </div>
  );
}
