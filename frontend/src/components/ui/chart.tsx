import * as React from "react";

interface ChartContainerProps {
  children: React.ReactNode;
  config: any;
  className?: string;
}

export function ChartContainer({ children, config, className }: ChartContainerProps) {
  return (
    <div className={className}>
      {children}
    </div>
  );
}

interface ChartTooltipProps {
  children: React.ReactNode;
  cursor?: boolean;
  content?: React.ComponentType<any>;
}

export function ChartTooltip({ children, cursor, content: Content }: ChartTooltipProps) {
  return <>{children}</>;
}

interface ChartTooltipContentProps {
  nameKey?: string;
  hideLabel?: boolean;
  [key: string]: any;
}

export function ChartTooltipContent({ nameKey, hideLabel, ...props }: ChartTooltipContentProps) {
  return null;
}
