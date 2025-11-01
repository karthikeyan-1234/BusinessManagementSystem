import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'LookUpIdIn',
   standalone: true, // ✅ optional, allows direct import into components if using standalone architecture
  pure: true, // ✅ ensures it only recalculates when inputs change
})
export class LookUpIdInPipe implements PipeTransform {

  transform(id: number, list: {id: number, name: string}[]): string|undefined {
    const match = list.find(x => x.id === id);
    return match? match.name : undefined
  }

}
