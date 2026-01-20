interface ReportSlottimeBooking{
    bookingDate: Date;
    merchType: string;
    dataType: string;
    slotTime: Date;
    quantity: number;
}

export {ReportSlottimeBooking};

interface ReportSlottimeBookingAllWhse{
    bookingDate: Date;
    merchType: string;
    dataType: string;
    slotTime: Date;
    quantity: number;
    warehouseCode: string;
}

export {ReportSlottimeBookingAllWhse};