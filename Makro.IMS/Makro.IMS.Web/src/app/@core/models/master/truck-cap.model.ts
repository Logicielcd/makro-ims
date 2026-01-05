interface TruckCap
{
    internalTruckId: number,
    warehouseCode: string,
    fullPl: number,
    operationType: string,
}

export { TruckCap}

interface TruckRule{
    internalTruckId: string,
    warehouse: string,
    qtyType: string,
    operationType: string,
}

export {TruckRule}