import * as React from "react";

interface ChartContainerProps {
	children: React.ReactNode;
	config: Record<string, unknown>;
	className?: string;
}

export function ChartContainer({ children, className }: ChartContainerProps) {
	return (
		<div className={className}>
			{children}
		</div>
	);
}

interface ChartTooltipProps {
	children: React.ReactNode;
	cursor?: boolean;
	content?: React.ComponentType<unknown>;
}

export function ChartTooltip({ children }: ChartTooltipProps) {
	return <>{children}</>;
}

interface ChartTooltipContentProps {
	nameKey?: string;
	hideLabel?: boolean;
	props?: Record<string, unknown>;
}

export function ChartTooltipContent(_: ChartTooltipContentProps) {
	return null;
}
