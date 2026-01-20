import { AbstractControl } from "@angular/forms";

export function ValidatePasswordConfirmation(control: AbstractControl,control2: AbstractControl){
    if (!control.value != control2.value){
        return { passwordnotMatch: true};
    }
    return null;
}