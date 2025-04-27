import React, { useState } from "react";
import { motion } from "framer-motion";
import { Tooltip } from "react-tooltip";
import "react-tooltip/dist/react-tooltip.css";

export interface GanttChartItem {
  [key: string]: any;
  id: string;
  name: string;
  startValue: number;
  endValue: number;
}

interface GanttChartProps {
  items: GanttChartItem[];
  title?: string;
  width?: number;
  height?: number;
  barHeight?: number;
  showLegend?: boolean;
  timeFormat?: (value: number) => string;
  customKeys?: string[];
  theme?: "light" | "dark";
  showAxis?: boolean;
  showTooltips?: boolean;
  barBorderRadius?: number;
  barHoverEffect?: boolean;
}

const GanttChart: React.FC<GanttChartProps> = ({
  items,
  title = "Gantt Chart",
  width = 800,
  height = 200,
  barHeight = 30,
  showLegend = true,
  timeFormat = (value) => value.toString(),
  customKeys,
  theme = "light",
  showAxis = true,
  showTooltips = true,
  barBorderRadius = 4,
  barHoverEffect = true,
}) => {
  // Calculate min and max values for scaling
  const minValue = Math.min(...items.map((item) => item.startValue));
  const maxValue = Math.max(...items.map((item) => item.endValue));
  const totalDuration = maxValue - minValue;
  const scaleFactor = width / totalDuration;

  // Enhanced color generator with memoization
  const getProcessColor = React.useMemo(() => {
    const colorCache = new Map<string, string>();

    return (id: string) => {
      if (colorCache.has(id)) return colorCache.get(id)!;

      // Improved color generation algorithm
      let hash = 0;
      for (let i = 0; i < id.length; i++) {
        hash = id.charCodeAt(i) + ((hash << 5) - hash);
      }

      const h = Math.abs(hash % 360);
      const s = 70 + (hash % 15); // Saturation between 70-85%
      const l = 50 + (hash % 20); // Lightness between 50-70%

      const color = `hsl(${h}, ${s}%, ${l}%)`;
      colorCache.set(id, color);
      return color;
    };
  }, []);

  // Format key names for display
  const formatKeyName = (key: string) => {
    return key
      .replace(/([A-Z])/g, " $1")
      .replace(/^./, (str) => str.toUpperCase());
  };

  // Get all keys to display in tooltip
  const getAllDisplayKeys = () => {
    if (customKeys) return customKeys;

    const keys = new Set<string>();
    items.forEach((item) => {
      Object.keys(item).forEach((key) => {
        if (!["id", "name", "startValue", "endValue"].includes(key)) {
          keys.add(key);
        }
      });
    });
    return Array.from(keys);
  };

  const displayKeys = getAllDisplayKeys();

  // State for hovered item
  const [hoveredItem, setHoveredItem] = useState<string | null>(null);

  return (
    <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
      <h2 className="text-xl font-semibold text-gray-800 dark:text-white mb-4">
        {title}
      </h2>

      <div className="overflow-x-auto">
        <div
          className="relative bg-gray-50 dark:bg-gray-700 rounded-lg p-4"
          style={{
            width: `${width}px`,
            height: `${height}px`,
          }}
        >
          {/* Timeline bars */}
          {items.map((item, index) => {
            const duration = item.endValue - item.startValue;
            const left = (item.startValue - minValue) * scaleFactor;
            const barWidth = Math.max(20, duration * scaleFactor);
            const color = getProcessColor(item.id);
            const isHovered = hoveredItem === item.id;

            return (
              <motion.div
                key={`${item.id}-${index}`}
                className="absolute flex items-center group"
                style={{
                  left: `${left}px`,
                  width: `${barWidth}px`,
                  height: `${barHeight}px`,
                  top: `${index * (barHeight + 12)}px`,
                  backgroundColor: color,
                  borderRadius: `${barBorderRadius}px`,
                  zIndex: isHovered ? 10 : 1,
                }}
                initial={{ opacity: 0.9 }}
                animate={{
                  opacity: isHovered ? 1 : 0.9,
                  scale: isHovered && barHoverEffect ? 1.02 : 1,
                  boxShadow:
                    isHovered && barHoverEffect
                      ? `0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)`
                      : "none",
                }}
                transition={{ duration: 0.2 }}
                onMouseEnter={() => setHoveredItem(item.id)}
                onMouseLeave={() => setHoveredItem(null)}
                data-tooltip-id={`tooltip-${item.id}`}
              >
                <div className="px-2 py-1 text-xs font-medium text-white truncate w-full text-center">
                  {item.name}
                </div>

                {/* Enhanced Tooltip */}
                {showTooltips && (
                  <Tooltip
                    id={`tooltip-${item.id}`}
                    place="top"
                    className="!bg-white dark:!bg-gray-700 !text-gray-800 dark:!text-gray-200 !p-4 !rounded-lg !shadow-xl !max-w-xs !border !border-gray-200 dark:!border-gray-600 z-50"
                  >
                    <div className="space-y-2">
                      <div className="font-bold text-sm">{item.name}</div>
                      <div className="flex items-center">
                        <div
                          className="w-3 h-3 rounded-sm mr-2"
                          style={{ backgroundColor: color }}
                        />
                        <span className="text-xs">
                          {timeFormat(item.startValue)} -{" "}
                          {timeFormat(item.endValue)}
                        </span>
                      </div>
                      <div className="grid grid-cols-2 gap-2 text-xs">
                        {displayKeys.map((key) => (
                          <React.Fragment key={key}>
                            <div className="font-semibold">
                              {formatKeyName(key)}:
                            </div>
                            <div>{item[key] ?? "N/A"}</div>
                          </React.Fragment>
                        ))}
                      </div>
                    </div>
                  </Tooltip>
                )}
              </motion.div>
            );
          })}

          {/* Timeline axis */}
          {showAxis && (
            <div className="absolute bottom-0 left-0 right-0 h-6 border-t border-gray-200 dark:border-gray-600">
              {/* Start label */}
              <div
                className="absolute text-xs text-gray-500 dark:text-gray-400"
                style={{ left: "0", bottom: "-20px" }}
              >
                {timeFormat(minValue)}
              </div>

              {/* End label */}
              <div
                className="absolute text-xs text-gray-500 dark:text-gray-400"
                style={{ right: "0", bottom: "-20px" }}
              >
                {timeFormat(maxValue)}
              </div>
            </div>
          )}
        </div>

        {/* Enhanced Legend */}
        {showLegend && (
          <div className="mt-8 border-t border-gray-200 dark:border-gray-700 pt-6">
            <h3 className="text-sm font-semibold text-gray-800 dark:text-gray-200 mb-3">
              LEGEND
            </h3>
            <div className="flex flex-wrap gap-3">
              {items.map((item) => {
                const color = getProcessColor(item.id);
                return (
                  <motion.div
                    key={item.id}
                    className={`flex items-center px-3 py-1.5 rounded-full bg-gray-50 dark:bg-gray-700 border border-gray-200 dark:border-gray-600 ${
                      hoveredItem === item.id ? "ring-2 ring-opacity-50" : ""
                    }`}
                    style={
                      {
                        borderColor: hoveredItem === item.id ? color : "",
                        "--ring-color": color,
                      } as React.CSSProperties
                    }
                    whileHover={{ scale: 1.05 }}
                    onMouseEnter={() => setHoveredItem(item.id)}
                    onMouseLeave={() => setHoveredItem(null)}
                  >
                    <div
                      className="w-3 h-3 rounded-sm mr-2"
                      style={{ backgroundColor: color }}
                    />
                    <span className="text-sm text-gray-700 dark:text-gray-300">
                      {item.name}
                    </span>
                  </motion.div>
                );
              })}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default GanttChart;
