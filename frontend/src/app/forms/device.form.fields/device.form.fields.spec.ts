import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceFormFields } from './device.form.fields';

describe('DeviceFormFields', () => {
  let component: DeviceFormFields;
  let fixture: ComponentFixture<DeviceFormFields>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeviceFormFields],
    }).compileComponents();

    fixture = TestBed.createComponent(DeviceFormFields);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
