import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ActionOnApprovalComponent } from './action-on-approval.component';

describe('ActionOnApprovalComponent', () => {
  let component: ActionOnApprovalComponent;
  let fixture: ComponentFixture<ActionOnApprovalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ActionOnApprovalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ActionOnApprovalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
