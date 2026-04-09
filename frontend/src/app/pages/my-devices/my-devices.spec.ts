import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyDevices } from './my-devices';

describe('MyDevices', () => {
  let component: MyDevices;
  let fixture: ComponentFixture<MyDevices>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyDevices],
    }).compileComponents();

    fixture = TestBed.createComponent(MyDevices);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
