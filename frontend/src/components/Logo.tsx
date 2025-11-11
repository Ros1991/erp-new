interface LogoProps {
  className?: string;
  showText?: boolean;
}

export function Logo({ className = "h-10", showText = true }: LogoProps) {
  return (
    <div className="flex items-center space-x-3">
      <svg 
        className={className}
        viewBox="0 0 200 200" 
        fill="none" 
        xmlns="http://www.w3.org/2000/svg"
      >
        {/* Bar Chart */}
        <rect x="40" y="120" width="30" height="40" rx="4" fill="#1e40af"/>
        <rect x="85" y="90" width="30" height="70" rx="4" fill="#1e40af"/>
        <rect x="130" y="60" width="30" height="100" rx="4" fill="#1e40af"/>
        
        {/* Growth Line */}
        <path d="M55 130 L100 100 L145 70" stroke="#10b981" strokeWidth="8" strokeLinecap="round" strokeLinejoin="round"/>
        
        {/* Arrow */}
        <path d="M135 65 L150 65 L150 80" stroke="#10b981" strokeWidth="8" strokeLinecap="round" strokeLinejoin="round"/>
        
        {/* Dots */}
        <circle cx="55" cy="130" r="8" fill="#10b981"/>
        <circle cx="100" cy="100" r="8" fill="#10b981"/>
        <circle cx="145" cy="70" r="8" fill="#10b981"/>
      </svg>
      
      {showText && (
        <div className="flex flex-col">
          <span className="text-2xl font-bold text-primary-800">Meu</span>
          <span className="text-2xl font-bold text-primary-800 -mt-1">Gestor</span>
        </div>
      )}
    </div>
  );
}
