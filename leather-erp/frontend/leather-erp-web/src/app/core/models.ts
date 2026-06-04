export type UnitOfMeasure = 'Dm2' | 'SquareFoot';

export interface LoginResponse {
  token: string;
  expiresAt: string;
  username: string;
  role: string;
}

export interface Supplier {
  id: string;
  name: string;
  phone?: string;
  email?: string;
  notes?: string;
}

export interface LeatherType {
  id: string;
  name: string;
  kind?: string;
  color?: string;
  thicknessMm?: number;
  lowStockThresholdDm2: number;
}

export interface LeatherLot {
  id: string;
  leatherTypeId: string;
  leatherType?: LeatherType;
  supplierId?: string;
  supplier?: Supplier;
  lotNumber?: string;
  purchaseDate: string;
  quantityDm2: number;
  remainingDm2: number;
  unitCostPerDm2: number;
}

export interface StockLevel {
  leatherTypeId: string;
  leatherTypeName: string;
  remainingDm2: number;
  stockValue: number;
  lowStockThresholdDm2: number;
  isLow: boolean;
}

export interface Recipe {
  id?: string;
  productId?: string;
  leatherTypeId: string;
  netLeatherDm2: number;
  wasteRate: number;
  laborCost: number;
  overheadCost: number;
}

export interface FinishedGoods {
  id: string;
  productId: string;
  quantityOnHand: number;
  averageUnitCost: number;
  totalValue: number;
  product?: Product;
}

export interface Product {
  id: string;
  name: string;
  sku?: string;
  category?: string;
  isActive: boolean;
  recipe?: Recipe;
  inventory?: FinishedGoods;
}

export interface ProductionOrder {
  id: string;
  productId: string;
  product?: Product;
  quantity: number;
  orderDate: string;
  status: 'Draft' | 'Confirmed' | 'Cancelled';
  unitCostSnapshot: number;
  totalLeatherConsumedDm2: number;
  notes?: string;
}

export interface AppSettings {
  id?: string;
  displayUnit: UnitOfMeasure;
  currencyCode: string;
  vatRate: number;
  defaultWasteRate: number;
  defaultProfitMargin: number;
}

export interface CostBreakdown {
  grossLeatherDm2: number;
  netMaterialCost: number;
  wasteCost: number;
  materialCost: number;
  laborCost: number;
  overheadCost: number;
  unitCost: number;
}

export interface PricingResult {
  unitCost: number;
  profitAmount: number;
  priceBeforeTax: number;
  vatAmount: number;
  salePrice: number;
}

export interface ProductPricing {
  productId: string;
  leatherAvgUnitCostPerDm2: number;
  availableLeatherDm2: number;
  cost: CostBreakdown;
  price: PricingResult;
  appliedProfitMargin: number;
  appliedVatRate: number;
}

export interface ReportSummary {
  totalLeatherStockDm2: number;
  leatherStockValue: number;
  finishedGoodsValue: number;
  finishedGoodsUnits: number;
  lowStockCount: number;
  productionOrderCount: number;
  unitsProduced: number;
  totalRevenue: number;
  totalProfit: number;
  unitsSold: number;
}

export interface ProductionTrendPoint {
  period: string;
  label: string;
  units: number;
  value: number;
  salesUnits: number;
  revenue: number;
}

export interface SalesOrder {
  id: string;
  productId: string;
  product?: Product;
  quantity: number;
  saleDate: string;
  unitPrice: number;
  unitCost: number;
  customerName?: string;
  notes?: string;
  revenue: number;
  profit: number;
}

export interface ProductProfitability {
  productId: string;
  productName: string;
  unitCost: number;
  salePrice: number;
  unitProfit: number;
  profitMargin: number;
  producibleUnits: number;
}
