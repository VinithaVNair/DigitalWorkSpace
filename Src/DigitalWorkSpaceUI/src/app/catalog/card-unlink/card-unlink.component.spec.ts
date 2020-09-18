import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardUnlinkComponent } from './card-unlink.component';

describe('CardUnlinkComponent', () => {
  let component: CardUnlinkComponent;
  let fixture: ComponentFixture<CardUnlinkComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardUnlinkComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardUnlinkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
