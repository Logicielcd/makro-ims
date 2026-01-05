interface ReportTruckStatus{
    rowNo: number;
    bookingId: string;
    bookingDate: Date;
    licensePlate: string;
    supCode: string;
    supName: string;
    merchType: string;
    totalWeight: number;
    slotBooking: string;
    door: string;
    queueSeq: string;
    status: string;
}

export {ReportTruckStatus};