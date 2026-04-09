import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceTable } from './device-table';

describe('DeviceTable', () => {
  let component: DeviceTable;
  let fixture: ComponentFixture<DeviceTable>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeviceTable],
    }).compileComponents();

    fixture = TestBed.createComponent(DeviceTable);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
