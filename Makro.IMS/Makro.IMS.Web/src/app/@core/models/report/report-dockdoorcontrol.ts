interface ReportDockDoorControl{
    doorName: string;
    doorArea: string;
    doorType: string;
    supCode: string;
    supName: string;
    licensePlate: string;
    truckType: string;
    assignQueueTime: Date;
    onDockTime: Date;
    processTime: number;
    sequence: string;
    status: string;
    statusIcon: string;
    processTimeDisplay: string;
}

export {ReportDockDoorControl};
